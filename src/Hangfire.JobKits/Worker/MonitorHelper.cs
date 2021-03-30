using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cronos;

namespace Hangfire.JobKits.Worker
{
    internal static class MonitorHelper
    {
        /// <summary>
        /// Gets the map.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public static MonitorMap GetMap(Assembly[] assemblies)
        {
            var jobMethods = assemblies
                .SelectMany(x => x.GetTypes())
                .SelectMany(y => y.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
                .Where(x => x.GetCustomAttribute<JobValidationAttribute>(true) != null);

            if (jobMethods.Count() == 0)
            {
                return null;
            }

            var jobMonitors = jobMethods.GetMonitorJobs()
                .GroupBy(x => x.Range)
                .ToDictionary(x => x.Key, x => x.OrderBy(y => y.MonitorTime).ToList());

            return new MonitorMap(jobMonitors);
        }

        public static IEnumerable<MonitorJob> GetMonitorJobs(this IEnumerable<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                var validationAttribute = method.GetCustomAttribute<JobValidationAttribute>(true);

                if (validationAttribute is null || string.IsNullOrEmpty(validationAttribute.Cron)) continue;
                
                var from = DateTime.Today.Date.ToUniversalTime();
                var to = from.GetNextRangeTime(validationAttribute.Range).ToUniversalTime();

                var cronExpression = CronExpression.Parse(validationAttribute.Cron);
                var occurences = cronExpression.GetOccurrences(from, to, TimeZoneInfo.Local);
                DateTime nextTime = from.AddDays(1);

                foreach (var occurrence in occurences.Reverse())
                {
                    yield return new MonitorJob(validationAttribute, method, occurrence, nextTime);

                    nextTime = occurrence;
                }
            }
        }

        private static DateTime GetNextRangeTime(this DateTime from, string range)
        {
            switch (range)
            {
                default:
                case ValidateRangeType.Daily:
                    return from.AddDays(1);

                case ValidateRangeType.Weekly:
                    return from.AddDays(7);

                case ValidateRangeType.Monthly:
                    return from.AddMonths(1);
            }
        }
    }
}