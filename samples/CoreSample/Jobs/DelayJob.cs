using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.Console;
using Hangfire.JobKits;
using Hangfire.Server;

namespace CoreSample.Jobs
{
    [JobLauncher(CategoryName = "Delay")]
    public class DelayJob
    {
        [JobSingleton]
        [JobMethod(Name = "Delay Job", RecurringJobId = "DelayJob_J01")]
        public void Delay(PerformContext context, int delaySec)
        {
            context.WriteLine($"Start delay: {delaySec} sec");

            Thread.Sleep(delaySec * 1000);

            context.WriteLine("Doen");

            context.WriteLine();
        }
    }
}
