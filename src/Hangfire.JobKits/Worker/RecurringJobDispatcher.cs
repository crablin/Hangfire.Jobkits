﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Common;
using Hangfire.Dashboard;

namespace Hangfire.JobKits.Worker
{
    internal sealed class RecurringJobDispatcher : IDashboardDispatcher
    {
        public StandbyMap Map { get; }

        public RecurringJobDispatcher(StandbyMap map)
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
                var key = context.Request.GetQuery(StandbyKey.IdField);

                var standbyJob = Map.JobCollection[key];

                var cron = (await context.Request.GetFormValuesAsync("recurring_cron")).LastOrDefault();

                var parameters = await StandbyHelper.CreateParameters(context, standbyJob.Method);

                context.GetRecurringJobManager().AddOrUpdate(standbyJob.RecurringJobId, new Job(standbyJob.Method, parameters), cron);
                context.Response.StatusCode = 200;

            }
            catch (Exception e)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(e.Message);
            }
        }
    }
}
