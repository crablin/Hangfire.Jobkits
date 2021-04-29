using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hangfire.Storage;

namespace Hangfire.JobKits.Worker
{
    internal static class MonitorJobExtensions
    {
        private static readonly object _lockFlag = new object();

        internal static string GetFullActionName(this MethodInfo method)
        {
            return $"{method.DeclaringType.Name}.{ method.Name}";
        }

        internal static string GetMonitorStateKey(this MonitorJob monitorJob)
        {
            return $"monitor-job:{monitorJob.MonitorTime.ToLocalTime().Date.Ticks}:{monitorJob.ActionName}";
        }

        internal static string GetMonitorStateKey(this IStorageConnection connection, DateTime date, string actionName)
        {
            return $"monitor-job:{date.ToLocalTime().Date.Ticks}:{actionName}";
        }

        internal static Dictionary<string, string> GetMonitorState(this IStorageConnection connection, string key)
        {
            var result = connection.GetAllEntriesFromHash(key);

            if (result == null)
                result = new Dictionary<string, string>();

            return result;
        }

        internal static void SetMonitorState(this IStorageConnection connection, string key, MonitorJobStatusDto dto)
        {
            lock (_lockFlag)
            {
                var source = connection.GetMonitorState(key);
                
                source.Add(DateTime.UtcNow.Ticks.ToString(), dto.ToString());

                connection.SetRangeInHash(key, source);
            }
        }

        internal static MonitorJobStatusDto GetStatus(this MonitorJob job, JobStorage storage)
        {
            long currentTicks = DateTime.UtcNow.Ticks;

            if (job.MonitorTime.Ticks > currentTicks)
                return new MonitorJobStatusDto(MonitorJobStatus.Unstarted);

            var key = job.GetMonitorStateKey();

            using (var connection = storage.GetConnection())
            {
                var source = connection.GetMonitorState(key);

                if (source == null) return new MonitorJobStatusDto(MonitorJobStatus.Unstarted);

                var startTick = job.MonitorTime.Ticks.ToString();
                var endTick = job.NextTime.Ticks.ToString();
                var lastKey = source.Keys.OrderBy(k => k)
                    .LastOrDefault(k => string.Compare(k, startTick) >= 0 && string.Compare(k, endTick) == -1);

                if (string.IsNullOrEmpty(lastKey) && job.NextTime.Ticks < currentTicks)
                {
                    return new MonitorJobStatusDto(MonitorJobStatus.Unexecuted);
                }
                else if (!string.IsNullOrEmpty(lastKey))
                {
                    var result = source[lastKey].Split(';');

                    return new MonitorJobStatusDto
                    {
                        Status = (MonitorJobStatus)Enum.Parse(typeof(MonitorJobStatus), result[0]),
                        ExecutedTime = new DateTime(long.Parse(result[1])),
                        ExecutedJobId = result[2]
                    };
                }
            }

            return new MonitorJobStatusDto(MonitorJobStatus.Unstarted);
        }
    }
}