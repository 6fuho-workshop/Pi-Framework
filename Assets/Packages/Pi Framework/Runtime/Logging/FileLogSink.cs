// =====================
// PF.Logging.FileSink (tùy chọn)
// =====================
//
// Mục tiêu:
// - Ghi file background để không chặn thread chính.
// - Batch theo thời gian/số lượng; flush khi dispose.
// - Đơn giản, không rotate (bạn có thể bổ sung sau).
//
// Lưu ý: gọi Dispose (hoặc cung cấp API Stop) khi app thoát để flush.

using System;
using System.Collections.Concurrent;
using PF.Logging.Internal;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#nullable enable
namespace PF.Logging
{
    public sealed class FileLogSink : ILogSink, IDisposable
    {
        /// <summary>
        /// nếu qúa giới hạn này thì sẽ xóa nội dung file để tránh quá tải.
        /// </summary>
        const long maxFileSize = 20 * 1024; // 20KB
        private readonly ILogFormatter _formatter;
        private readonly BlockingCollection<string> _queue = new(boundedCapacity: 1024);
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _worker;
        private readonly string _path;
        private readonly FormatOptions _options = new() { 
            AddTime = true, 
            TimeFormat = "yy-MM-dd'T'HH:mm:ss.fff'Z'", 
            UseUtc = true
        };
        private bool _disposed = false;

        public FileLogSink(string path, ILogFormatter? formatter = null)
        {
            _path = path;
            _formatter = formatter ?? new SimpleTextFormatter();
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path)) ?? ".");
            _worker = Task.Run(WorkerLoop);
        }

        public string GetExceptionMsg(string category, Exception ex)
        {
            // Log thông tin cơ bản về exception
            string message = $"Exception occurred in {category}: {ex.Message}";

            message += $"\n{ex.StackTrace}";

            var st = new StackTrace(ex, true); // true => lấy cả số dòng
            var frames = st.GetFrames();
            if (frames != null)
            {
                foreach (var frame in frames)
                {
                    var method = frame.GetMethod();
                    if (method != null)
                    {
                        message += $"{method.DeclaringType}.{method.Name} (Line {frame.GetFileLineNumber()})";
                    }
                }
            }

            return message;

        }

        public void Write(in LogEvent e)
        {
            // Suy luận nhanh: nếu queue full -> drop dịu dàng để không block
            var line = _formatter.Format(in e, _options);
            if(e.Exception is not null)
            {
                // Nếu có exception, ghi rõ thông tin lỗi
                line += "\n" + GetExceptionMsg(e.Category, e.Exception);
            }
            if (!_queue.IsAddingCompleted)
                _queue.TryAdd(line);
        }

        private void WorkerLoop()
        {
            try
            {
                // Check file size and clear content if needed (only once, before processing queue)
                if (File.Exists(_path) && new FileInfo(_path).Length > maxFileSize)
                {
                    try
                    {
                        using (var clearFs = new FileStream(_path, FileMode.Truncate, FileAccess.Write, FileShare.Read)) { }
                    }
                    catch
                    {
                        // If clearing fails, continue with current file
                    }
                }

                using var fs = new FileStream(_path, FileMode.Append, FileAccess.Write, FileShare.Read);
                using var sw = new StreamWriter(fs, new UTF8Encoding(false)) { AutoFlush = false };

                var lastFlush = DateTime.UtcNow;
                foreach (var line in _queue.GetConsumingEnumerable(_cts.Token))
                {
                    sw.WriteLine(line);

                    // Flush theo chu kỳ ~2s để cân bằng độ an toàn và hiệu năng
                    if ((DateTime.UtcNow - lastFlush).TotalSeconds >= 2)
                    {
                        sw.Flush();
                        lastFlush = DateTime.UtcNow;
                    }
                }

                sw.Flush();
            }
            catch (OperationCanceledException) { /* normal */ }
            catch (IOException)
            {
                // Khi gặp lỗi IO, dừng ghi log
                _queue.CompleteAdding();
            }
            catch
            {
                // swallow IO errors to avoid crashing app
            }

            // Nếu bị lỗi IO, không restart worker nữa
            if (!_cts.Token.IsCancellationRequested && !_queue.IsCompleted)
            {
                Task.Run(WorkerLoop);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _queue.CompleteAdding();
            _cts.Cancel();
            try { _worker.Wait(2000); } catch { /* ignore */ }
            _cts.Dispose();
            _queue.Dispose();
        }
    }
}
