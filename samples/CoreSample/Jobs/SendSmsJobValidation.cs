using System;
using Hangfire.JobKits;

namespace CoreSample.Jobs
{

    public class SendSmsJobValidationAttribute : JobValidationAttribute
    {

        public SendSmsJobValidationAttribute() : base(typeof(SendSmsnJobValidationFilter))
        { }

        private class SendSmsnJobValidationFilter : JobValidationStateFilter
        {
            private IServiceProvider _serviceProvider;

            public SendSmsnJobValidationFilter(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public override bool Validate()
            {
                return true;
            }
        }

    }
}