using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hangfire.Dashboard;
using Hangfire.JobKits.Resources;
using Hangfire.Server;
using Newtonsoft.Json;

namespace Hangfire.JobKits.Worker
{
    internal static class StandbyHelper
    {
        /// <summary>
        /// Gets the map.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public static StandbyMap GetMap(Assembly[] assemblies)
        {
            var jobLauncherTypes = assemblies
                .SelectMany(x => x.GetTypes())
                .Where(x => x.GetCustomAttribute<JobLauncherAttribute>() != null);

            if (jobLauncherTypes.Count() == 0)
            {
                return null;
            }

            var jobMethods = jobLauncherTypes.GetJobMethods()
                .OrderBy(x => x.Name)
                .ToDictionary(x => x.Id, x => x);

            if (jobMethods.Count == 0)
            {
                return null;
            }

            return new StandbyMap(jobMethods);
        }

        private static IEnumerable<StandbyJob> GetJobMethods(this IEnumerable<Type> targetTypes)
        {
            foreach (var targetType in targetTypes)
            {
                var launcherAttribute = targetType.GetCustomAttribute<JobLauncherAttribute>();

                var methods = targetType
                    .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                    .Where(x => x.GetCustomAttribute<JobMethodAttribute>() != null);

                foreach (var method in methods)
                {
                    var methodAttribute = method.GetCustomAttribute<JobMethodAttribute>();
                    yield return new StandbyJob(launcherAttribute, methodAttribute, method);
                }
            }
        }

        /// <summary>
        /// Creates the parameters.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<object[]> CreateParameters(DashboardContext context, MethodInfo method)
        {
            var result = new List<object>();
            var fields = new List<string>();

            foreach (var parameter in method.GetParameters())
            {
                var parameterType = parameter.ParameterType;
                var parameterValue = (await context.Request.GetFormValuesAsync(parameter.Name)).LastOrDefault();

                switch (parameterValue)
                {
                    case var _ when parameterType == typeof(PerformContext):
                    case var _ when parameterType == typeof(IJobCancellationToken):
                        result.Add(null);
                        break;

                    case string stringValue when parameterType == typeof(string):
                        result.Add(stringValue);
                        break;

                    case string dateTimeValue when parameterType == typeof(DateTime):
                        var parsed = DateTime.TryParse(dateTimeValue, out var dateTime);
                        if (parsed)
                        {
                            result.Add(dateTime);
                        }
                        else
                        {
                            fields.Add(parameter.Name);
                        }
                        break;

                    case string enumString when parameterType.IsEnum:
                        result.Add(Enum.Parse(parameterType, enumString));
                        break;

                    case string primitiveValue when !string.IsNullOrWhiteSpace(parameterValue):
                        result.Add(JsonConvert.DeserializeObject(primitiveValue, parameterType));
                        break;

                    default:
                        fields.Add(parameter.Name);
                        continue;
                }
            }

            if (fields.Count > 0)
                throw new InvalidOperationException($"{Strings.Field_Missing}{string.Join(",", fields)}");

            return result.ToArray();
        }
    }
}
