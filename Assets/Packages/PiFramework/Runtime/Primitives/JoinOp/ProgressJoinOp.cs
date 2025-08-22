using PF.Utils;
using System.Collections.Generic;

namespace PF.Primitives
{
    public class ProgressJoinOp : ProgressJoinOp<PercentProgressReport> { }

    public class ProgressJoinOp<TReport> : JoinOp where TReport : IProgressJoinOpReport
    {
        public readonly PiProgress<TReport> Progress;

        // Store the latest report per participant
        protected readonly Dictionary<object, TReport> _reports = new();

        public ProgressJoinOp()
        {
            Progress = new PiProgress<TReport>(Handle);
        }

        protected virtual void Handle(TReport report)
        {
            report.ThrowIfNull().Participant.ThrowIfNull();

            // Check if participant is registered
            lock (syncRoot)
            {
                if (participants.Contains(report.Participant))
                {
                    // Update or add the latest report for this participant
                    _reports[report.Participant] = report;
                }
            }
        }

        protected override void OnUnregister(object participant)
        {
            lock (syncRoot)
            {
                // Remove the report for this participant
                _reports.Remove(participant);
            }
            base.OnUnregister(participant);
        }
    }
}