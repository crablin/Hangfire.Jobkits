using System;

namespace Hangfire.JobKits.Worker
{
    public enum MonitorJobStatus
    {
        Successed,
        Failed,
        Invalid,
        Unexecuted,
        Unstarted
    }

    public class MonitorJobStatusDto
    {
        public MonitorJobStatusDto()
        {
        }

        public MonitorJobStatusDto(MonitorJobStatus status)
        {
            Status = status;
        }

        public MonitorJobStatus Status { get; set; }
        public DateTime? ExecutedTime { get; set; }
        public string ExecutedJobId { get; set; }

        public override string ToString()
        {
            //$"{jobStatus};{currentTicks};{jobId}"
            var ticks = ExecutedTime.HasValue ? ExecutedTime.Value.Ticks.ToString() : string.Empty;

            return $"{Status};{ticks};{ExecutedJobId}";
        }

    }
}