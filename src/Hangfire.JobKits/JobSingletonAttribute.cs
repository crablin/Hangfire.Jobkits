using System;
using System.Linq;
using Hangfire;
using Hangfire.Annotations;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.JobKits.Worker;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;

namespace Hangfire.JobKits
{
    [PublicAPI]

    public class JobSingletonAttribute : JobFilterAttribute, IClientFilter
    {
        public void OnCreated(CreatedContext filterContext)
        {

        }

        public void OnCreating(CreatingContext filterContext)
        {

            if (IsRunning(filterContext.Storage.GetMonitoringApi(), filterContext.Job.Method.GetFullActionName()))
            {
                throw new Exception("This job cannot trigger multiple.");
            }

        }

        /// <summary>
        /// check Job is enqueued or processing
        /// </summary>
        /// <param name="actionName">full action name</param>
        /// <returns></returns>
        public static bool IsRunning(IMonitoringApi api, string actionName)
        {
            var queues = api.Queues();

            foreach (var queue in queues)
            {
                var queueName = queue.Name;
                var enqueuedCount = Convert.ToInt32(api.EnqueuedCount(queueName));
                var enqueuedJobs = api.EnqueuedJobs(queueName, 0, enqueuedCount);

                if (enqueuedJobs.Any(job => job.Value.Job.Method.GetFullActionName() == actionName)) return true;
            }

            var processingCount = Convert.ToInt32(api.ProcessingCount());
            var processingJobs = api.ProcessingJobs(0, processingCount);

            return processingJobs.Any(job => job.Value.Job.Method.GetFullActionName() == actionName);
        }

        
    }
}