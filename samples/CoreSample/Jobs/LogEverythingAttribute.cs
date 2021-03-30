using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Logging;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;

namespace CoreSample.Jobs
{
    public class LogEverythingAttribute : JobFilterAttribute,
     IClientFilter, IServerFilter, IElectStateFilter, IApplyStateFilter
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        void IClientFilter.OnCreating(CreatingContext context)
        {
            Logger.InfoFormat("Kevin Test");
        }

        void IClientFilter.OnCreated(CreatedContext context)
        {
            Logger.InfoFormat("Kevin Test");
        }

        void IServerFilter.OnPerforming(PerformingContext context)
        {
            Logger.InfoFormat("Kevin Test");
        }

        void IServerFilter.OnPerformed(PerformedContext context)
        {
            Logger.InfoFormat("Kevin Test");
        }

        void IElectStateFilter.OnStateElection(ElectStateContext context)
        {
            Logger.InfoFormat("Kevin Test");
        }

        void IApplyStateFilter.OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            Logger.InfoFormat("Kevin Test");
        }

        void IApplyStateFilter.OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            Logger.InfoFormat("Kevin Test");
        }
    }
}