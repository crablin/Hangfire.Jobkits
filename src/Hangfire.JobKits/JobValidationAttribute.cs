using System;
using System.Collections.Generic;
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

                    SetState(context, isValid ? MonitorJobStatus.Successed : MonitorJobStatus.Invalid);
                }
                else if (context.CandidateState is FailedState state)
                {
                    SetState(context, MonitorJobStatus.Failed);
                }
            }

            private void SetState(ElectStateContext context, MonitorJobStatus jobStatus)
            {
                var method = context.BackgroundJob.Job.Method;
                var key = $"monitor-job:{DateTime.Now.Date.Ticks}:{method.DeclaringType.FullName}.{method.Name}";

                lock (_lockFlag)
                {
                    var source = context.Connection.GetAllEntriesFromHash(key);

                    if (source == null)
                        source = new Dictionary<string, string>();

                    source.Add(
                        DateTime.Now.Ticks.ToString(),
                        $"{jobStatus};{DateTime.Now.Ticks};{context.BackgroundJob.Id}");

                    context.Connection.SetRangeInHash(key, source);
                }

            }
        }
    }
}