using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Hangfire.JobKits.Worker
{
    public enum MonitorJobStatus
    {
        Successed,
        Failed,
        Invalid,
        Unqueued,
        Wait
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
    }

    public class MonitorJob
    {
        public string Id { get; }
        public string Cron { get; }
        public DateTime MonitorTime { get; }
        public DateTime NextTime { get; }
        public string Range { get; set; }
        public string Name { get; }
        public MethodInfo Method { get; }
        public string ActionName { get; }

        public MonitorJobStatusDto GetStatus(JobStorage storage)
        {
            if (MonitorTime <= DateTime.Now)
                return new MonitorJobStatusDto(MonitorJobStatus.Wait);

            var key = $"monitor-job:{MonitorTime.Date.Ticks}:{ActionName}";

            using (var connection = storage.GetConnection())
            {
                var source = connection.GetAllEntriesFromHash(key);

                if (source == null) return new MonitorJobStatusDto(MonitorJobStatus.Wait);

                var startTick = MonitorTime.Ticks.ToString();
                var endTick = NextTime.Ticks.ToString();
                var lastKey = source.Keys.OrderBy(k => k)
                    .LastOrDefault(k => string.Compare(k, startTick) >= 0 && string.Compare(k, endTick) == -1);

                if (string.IsNullOrEmpty(lastKey) && NextTime < DateTime.Now)
                {
                    return new MonitorJobStatusDto(MonitorJobStatus.Unqueued);
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
            return new MonitorJobStatusDto(MonitorJobStatus.Wait);
        }

        public MonitorJob(
            JobValidationAttribute vaildateAttribute,
            MethodInfo method,
            DateTime monitorTime,
            DateTime nextTime
            )
        {
            Id = GenerateId(method);

            Name = vaildateAttribute.Name;
            Cron = vaildateAttribute.Cron;
            Range = vaildateAttribute.Range;

            Method = method;
            ActionName = $"{method.DeclaringType.Name}.{method.Name}";

            MonitorTime = monitorTime;
            NextTime = nextTime;
        }

        private static string GenerateId(MethodInfo method)
        {
            var id = GenerateSignature(method);

            using (var crypt = new SHA1Managed())
            {
                var hashStringBuilder = new StringBuilder();
                var hash = crypt.ComputeHash(Encoding.ASCII.GetBytes(id));
                foreach (var @byte in hash)
                {
                    hashStringBuilder.Append(@byte.ToString("x2"));
                }
                return hashStringBuilder.ToString();
            }
        }

        private static string GenerateSignature(MethodInfo method)
        {
            var declaringType = method.DeclaringType?.FullName ?? "Unknown";
            var methodName = method.Name;
            var parameterList = string.Join(", ", method.GetParameters().Select(x => $"{x.ParameterType.FullName}"));

            return $"{declaringType}.{methodName}({parameterList})";
        }
    }
}