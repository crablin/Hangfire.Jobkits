using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Hangfire.JobKits.Worker
{

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
            ActionName = method.GetFullActionName();

            

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