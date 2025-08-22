using System;

namespace PF.Primitives
{
    /// <summary>
    /// Represents percentage progress (0 to 100).
    /// </summary>
    public class PercentProgressReport : IProgressJoinOpReport
    {
        public object Participant { get; protected set; }

        private float _percentage = 0f;

        public string TaskName { get; protected set; }

        public string StatusMessage = string.Empty;

        /// <summary>
        /// Đã có auto clamp về khoảng [0, 100].
        /// </summary>
        public float Percentage
        {
            get => _percentage;
            set => _percentage = Math.Max(0f, Math.Min(100f, value));
        }

        /// <summary>
        /// Initializes a new PercentProgressReport with the specified participant and task name.
        /// </summary>
        /// <param name="participant">The reporting participant (must not be null).</param>
        /// <param name="taskName">The name of the task (must not be null).</param>
        public PercentProgressReport(object participant, string taskName)
        {
            Participant = participant ?? throw new ArgumentNullException(nameof(participant));
            TaskName = taskName ?? throw new ArgumentNullException(nameof(taskName));
        }
    }
}