namespace PF.Contracts
{
    /// Debug – log cho dev, giúp gỡ lỗi.
    /// Verbose – chi tiết hơn Info nhưng ít spam hơn Debug, thường để “mổ xẻ” quy trình.
    /// Info – thông tin chung, quan trọng nhưng không báo lỗi.
    /// Warning – cảnh báo có thể gây lỗi nhưng vẫn chạy được.
    /// Error – lỗi nghiêm trọng ảnh hưởng tính năng, nhưng chưa crash.
    /// Fatal – lỗi nghiêm trọng nhất, khiến game phải dừng hoặc reset.
    /// có thể cấu hình log filter theo môi trường:
    /// Development build: bật từ Debug trở lên(để debug tối đa).
    /// Production build: bật từ Error trở lên(để không flood log).
    /// QA/Testing build: bật từ Debug trở lên
    public enum LogLevel
    {
        Inherit = 100,   // CHỈ dành cho cấu hình (SO/JSON). Không set vào MinLevel runtime.
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warn = 3,
        Error = 4,
        Fatal = 5,
        None = 99
    }
}