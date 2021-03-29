using CoreSample.Jobs.DI;
using Hangfire.JobKits.Worker;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CoreSample.Jobs
{
    public class JobValidation : IJobValidation
    {
        private readonly IServiceProvider _serviceProvider;

        public JobValidation(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public bool Validation(string recurringJobId, PerformContext context)
        {
            //using (var scope = _serviceProvider.CreateScope())
            //{
            //    var sendProgress = scope.ServiceProvider.GetRequiredService<ISendProgress>();
            //    var instance = new SendMailJob2(sendProgress);
            //    bool isOK = instance.Send(context);
            //    isOK = true;
            //    return isOK;
            //}


            return true;
        }
    }
}