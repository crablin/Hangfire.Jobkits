using Hangfire.Client;
using Hangfire.Common;
using Hangfire.JobKits;
using Hangfire.Logging;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreSample.Jobs
{
    public class LogEverythingAttribute : JobValidationAttribute,
     IClientFilter, IServerFilter, IElectStateFilter, IApplyStateFilter
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        public void OnCreating(CreatingContext context)
        {
            Logger.InfoFormat("Kevin Test");
        }

        public void OnCreated(CreatedContext context)
        {
            Logger.InfoFormat("Kevin Test");

        }

        public void OnPerforming(PerformingContext context)
        {
            Logger.InfoFormat("Kevin Test");

        }

        public void OnPerformed(PerformedContext context)
        {
            Logger.InfoFormat("Kevin Test");

        }

        public void OnStateElection(ElectStateContext context)
        {
            Logger.InfoFormat("Kevin Test");

        }

        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            Logger.InfoFormat("Kevin Test");

        }

        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            Logger.InfoFormat("Kevin Test");

        }
    }
}
