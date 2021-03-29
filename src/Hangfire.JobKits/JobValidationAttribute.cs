using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire.JobKits.DataBase;
using Hangfire.Annotations;

namespace Hangfire.JobKits
{
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Method)]
    public class JobValidationAttribute : JobFilterAttribute, IClientFilter, IServerFilter, IElectStateFilter, IApplyStateFilter
    {
        private PerformedContext filterContext;
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            if (context.NewState.Name == "Succeeded")
            {
                var recurringJobId = context.Job.Method.CustomAttributes.FirstOrDefault(o => o.AttributeType.Name == "JobMethodAttribute").NamedArguments.ToList().Where(x => x.MemberName == "RecurringJobId").First().TypedValue;

                var job = new Dictionary<string, string>
                         {
                            {"LastExecutionStatus",true.ToString() },
                            {"LastExecutionTime",DateTime.UtcNow.ToLocalTime().ToString() },
                            {"LastExecutionJobId",context.JobId }

                         };
                Update(recurringJobId.Value.ToString(), job, context);
                ValidationJob(recurringJobId.Value.ToString(), context);
            }
        }
        public void ValidationJob(string recurringJobId, ApplyStateContext context)
        {
            var today = DateTime.Today;
            var year = today.Year;
            var month = today.Month;
            var day = today.Day;
            var ExcuteStartTime = DateTime.Parse(year + "-" + month + "-" + day + $" {StartHour}:{StartMinute}:00").ToUniversalTime();
            var ExcuteEndTime = DateTime.Parse(year + "-" + month + "-" + day + $" {EndHour}:{EndMinute}:59").ToUniversalTime();
            bool IsValidation = false;
            if (DateTime.UtcNow >= ExcuteStartTime && DateTime.UtcNow <= ExcuteEndTime)
            {
                if (GlobalJobValidations.Validation.Validation(recurringJobId, filterContext))
                {
                    var job = new Dictionary<string, string>
                         {
                            { "ValidationDateTime",DateTime.UtcNow.ToLocalTime().ToString()},
                            { "ValidationStatus", true.ToString() }
                         };
                    Update(recurringJobId, job, context);
                }
            }
        }
        private void Update(string recurringJobId, Dictionary<string, string> jobs, ApplyStateContext context)
        {
            var storage = context.Storage;
            using (var conn = storage.GetConnection())
            {
                conn.SetRangeInHash($"recurring-Monitor:{recurringJobId}", jobs);
            }
        }
        public void OnCreated(CreatedContext filterContext) { }
        public void OnCreating(CreatingContext filterContext) { }
        public void OnPerformed(PerformedContext filterContext) { this.filterContext = filterContext; }
        public void OnPerforming(PerformingContext filterContext) { }
        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction) { }
        public void OnStateElection(ElectStateContext context) { }
    }
}
