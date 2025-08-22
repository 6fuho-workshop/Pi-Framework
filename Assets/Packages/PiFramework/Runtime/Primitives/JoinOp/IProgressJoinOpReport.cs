namespace PF.Primitives
{
    /// <summary>
    /// Interface for progress report objects used in ProgressJoinOp.
    /// Implement this interface to allow a report to be associated with a participant (source).
    /// The Participant property should uniquely identify the owner or source of the report
    /// (for example, a module, task, or object reporting its progress).
    /// This enables ProgressJoinOp to track, validate, and aggregate progress from multiple participants.
    /// </summary>
    public interface IProgressJoinOpReport
    {
        /// <summary>
        /// The participant (source) associated with this report.
        /// Should be unique for each reporting entity.
        /// </summary>
        object Participant { get; }
        // Add other properties as needed to describe the report.
    }
}