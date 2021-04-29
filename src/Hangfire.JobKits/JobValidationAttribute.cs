using System;
using Hangfire.Annotations;
using Hangfire.JobKits.Providers;
using Hangfire.JobKits.Worker;
using Hangfire.States;

namespace Hangfire.JobKits
{
    public struct ValidateRangeType
    {
        public const string Daily = "Daily";
        public const string Weekly = "Weekly";
        public const string Monthly = "Monthly";
    }

    [PublicAPI]
    [AttributeUsage(AttributeTargets.Method)]
    public class JobValidationAttribute : TypeJobFilterAttribute
    {
        public string Name { get; set; }
        public string Cron { get; set; }
        public string Range { get; set; } = ValidateRangeType.Daily;

        public JobValidationAttribute()
            : this(typeof(JobValidationStateFilter))
        {
        }

        public JobValidationAttribute(Type typeFilter) : base(typeFilter)
        {
        }

        public class JobValidationStateFilter : IElectStateFilter
        {
            private object _lockFlag = new object();

            public virtual bool Validate() => true;

            void IElectStateFilter.OnStateElection(ElectStateContext context)
            {
                if (context.CandidateState is SucceededState succeededstate)
                {
                    var isValid = Validate();

                    var dto = new MonitorJobStatusDto
                    {
                        Status = isValid ? MonitorJobStatus.Successed : MonitorJobStatus.Invalid,
                        ExecutedTime = DateTime.UtcNow,
                        ExecutedJobId = context.BackgroundJob.Id
                    };

                    SetState(context, dto);
                }
                else if (context.CandidateState is FailedState state)
                {
                    SetState(context, new MonitorJobStatusDto(MonitorJobStatus.Failed));
                }
            }

            private void SetState(ElectStateContext context, MonitorJobStatusDto dto)
            {
                
                var actionName = context.BackgroundJob.Job.Method.GetFullActionName();
                var key = context.Connection.GetMonitorStateKey(DateTime.Today, actionName);

                context.Connection.SetMonitorState(key, dto);
            }
        }
    }
}