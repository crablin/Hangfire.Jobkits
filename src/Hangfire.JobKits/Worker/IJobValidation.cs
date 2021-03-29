using Hangfire.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hangfire.JobKits.Worker
{
    public interface IJobValidation
    {
        bool Validation(string recurringJobId, PerformContext context);
    }
}
