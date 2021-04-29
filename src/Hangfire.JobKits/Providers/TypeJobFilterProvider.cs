using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hangfire.Common;

namespace Hangfire.JobKits.Providers
{
    public class TypeJobFilterProvider : IJobFilterProvider
    {
        private readonly IServiceProvider serviceProvider;

        public TypeJobFilterProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IEnumerable<JobFilter> GetFilters(Job job)
        {
            var typeFilters = GetTypeJobFilterAttributes(job.Type)
                .Select(typeFilter => typeFilter.CreateInstance(serviceProvider))
                .Select(jobFilter => new JobFilter(jobFilter, JobFilterScope.Type, null));
            var methodFilters = GetTypeJobFilterAttributes(job.Method)
                .Select(typeFilter => typeFilter.CreateInstance(serviceProvider))
                .Select(jobFilter => new JobFilter(jobFilter, JobFilterScope.Method, null));

            return typeFilters.Concat(methodFilters);
        }

        private static IEnumerable<TypeJobFilterAttribute> GetTypeJobFilterAttributes(MemberInfo memberInfo)
        {
            return memberInfo
                .GetCustomAttributes(typeof(TypeJobFilterAttribute), inherit: true)
                .Cast<TypeJobFilterAttribute>();
        }
    }
}
