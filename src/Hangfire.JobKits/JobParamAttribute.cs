using System;
using Hangfire.Annotations;

namespace Hangfire.JobKits
{

    [PublicAPI]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class JobParamAttribute : Attribute
    {
        public object DefaultValue { get; set; }
        public string Description { get; set; }
    }
}
