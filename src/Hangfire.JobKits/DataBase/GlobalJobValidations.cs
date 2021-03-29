using Hangfire.JobKits.Worker;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hangfire.JobKits.DataBase
{
    public static class GlobalJobValidations
    {
        public static IJobValidation Validation { get; set; }
    }
}
