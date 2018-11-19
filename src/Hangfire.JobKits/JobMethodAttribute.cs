using System;
using Hangfire.Annotations;

namespace Hangfire.JobKits
{
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Method)]
    public class JobMethodAttribute : Attribute
    {
        /// <summary>
        /// Job name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// "critical" or "default"
        /// </summary>
        public string Queue { get; set; }
        /// <summary>
        /// Job description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// recurring job identifier.
        /// </summary>
        public string RecurringJobId { get; set; }

        /// <summary>
        /// recurring job CRON. (default)
        /// </summary>
        public string RecurringJobCron { get; set; }
    }
}
