using System;
using Hangfire.Annotations;

namespace Hangfire.JobKits
{

    [PublicAPI]
    [AttributeUsage(AttributeTargets.Parameter)]
    public class JobParamAttribute : Attribute
    {
        public JobParamAttribute()
        {

        }

        public JobParamAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        /// default value.
        /// </summary>
        public object DefaultValue { get; set; }
        /// <summary>
        /// description.
        /// </summary>
        public string Description { get; set; }
    }
}
