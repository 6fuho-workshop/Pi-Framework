namespace PF.Logging.Internal
{
    /// <summary>
    /// Represents a log sink that receives log events for processing and output.
    /// Implementations must ensure thread-safety if required.
    /// </summary>
    public interface ILogSink
    {
        // Sink tự đảm bảo thread-safety nội bộ nếu cần.

        /// <summary>
        /// Writes a log event to the sink.
        /// </summary>
        /// <param name="e">The log event to write.</param>
        void Write(in LogEvent e);
    }
}