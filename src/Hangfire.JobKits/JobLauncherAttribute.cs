using System;
using Hangfire.Annotations;

namespace Hangfire.JobKits
{
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class)]
    public class JobLauncherAttribute : Attribute
    {
        public string CategoryName { get; set; }

        public JobLauncherAttribute()
        {

        }

        public JobLauncherAttribute(string categoryName)
        {
            CategoryName = categoryName;
        }
    }
}
