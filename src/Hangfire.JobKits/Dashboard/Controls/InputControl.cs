using System;
using System.Reflection;
using Hangfire.Dashboard;
using Hangfire.JobKits.Dashboard.Controls.InputBox;
using Hangfire.JobKits.Worker;
using Hangfire.Server;

namespace Hangfire.JobKits.Dashboard.Controls
{
    internal static class InputControl
    {
        public static RazorPage CreateControl(ParameterInfo parameter, StandbyJob job)
        {
            var parameterType = parameter.ParameterType;
            switch (parameterType)
            {
                case var _ when parameterType == typeof(PerformContext):
                case var _ when parameterType == typeof(IJobCancellationToken):
                    return new NullControl();

                case var _ when parameterType == typeof(DateTime):
                    return new DateTimeControl(parameter);

                case var _ when parameterType == typeof(byte):
                case var _ when parameterType == typeof(int):
                case var _ when parameterType == typeof(long):
                case var _ when parameterType == typeof(float):
                case var _ when parameterType == typeof(double):
                    return new NumberControl(parameter);

                case var _ when parameterType == typeof(bool):
                    return new BooleanControl(parameter);

                case var _ when parameterType == typeof(string):
                    return new StringControl(parameter);

                default:
                    return new StringControl(parameter);
            }
        }
    }
}
