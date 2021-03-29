using System;
using System.Collections.Generic;
using System.Text;

namespace Hangfire.JobKits.Worker
{
    public class JobDto 
    {
        public string Id { get; set; }
        public string RecurringJobId { get; set; }
        public string JobName { get; set; }
        public string ExecutionStatus { get; set; }
        public string ExecutionTime { get; set; }
        public string ValidationStatus { get; set; }
        public string ValidationTime { get; set; }
        public string HrefString { get; set; }
    }
}
