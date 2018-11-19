using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.Server;
using Hangfire.States;
using Newtonsoft.Json;

namespace Hangfire.JobKits.Worker
{
    internal sealed class BackgroundJobDispatcher : IDashboardDispatcher
    {
        public StandbyMap Map { get; }

        public BackgroundJobDispatcher(StandbyMap map)
        {
            Map = map;
        }

        public async Task Dispatch(DashboardContext context)
        {
            if (!"POST".Equals(context.Request.Method, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Response.StatusCode = 405;
                return;
            }

            try
            {
                var infoKey = context.Request.GetQuery(StandbyKey.IdField);

                var standbyJob = Map.JobCollection[infoKey];

                var parameters = await StandbyHelper.CreateParameters(context, standbyJob.Method);
                var jobId = context.GetBackgroundJobClient().Create(new Job(standbyJob.Method, parameters), new EnqueuedState());
                
                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(jobId);
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(e.Message);
            }
        }

        
    }
}
