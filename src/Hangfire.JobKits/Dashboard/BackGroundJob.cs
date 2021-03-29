using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.Dashboard;
using Hangfire.JobKits.Worker;
using Hangfire.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hangfire.JobKits.Dashboard
{
    public class BackGroundJob : BackgroundService
    {
        private readonly IServiceProvider _services;
        public BackGroundJob(IServiceProvider services)
        {
            _services = services;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var today = DateTime.Today;
                var year = today.Year;
                var month = today.Month;
                var day = today.Day;
                var ResetStartTime = DateTime.Parse(year + "-" + month + "-" + day + " 00:00:00").ToUniversalTime();
                var ResetEndTime = DateTime.Parse(year + "-" + month + "-" + day + " 00:10:59").ToUniversalTime();

                if (DateTime.UtcNow >= ResetStartTime && DateTime.UtcNow <= ResetEndTime)
                {
                    using (var service = _services.CreateScope())
                    {
                        var storage = service.ServiceProvider.GetRequiredService<JobStorage>();
                        using (var conn = storage.GetConnection())
                        {
                            var data = conn.GetAllEntriesFromHash("Key");
                            var jobs = conn.GetRecurringJobs(data.Select(o => o.Value).ToList());
                            Parallel.ForEach(jobs.GetMonitorDefaultValue(), job =>
                            {
                                Console.WriteLine(job.Key);
                                conn.SetRangeInHash($"recurring-Monitor:{job.Key.ToString()}", job.Value);
                                conn.SetRangeInHash("Key", new Dictionary<string, string> { { $"RecurringJobId_{ job.Key }", job.Key } });
                            });
                        }
                    }
                }
                Thread.Sleep(600000);
            }

            return Task.CompletedTask;
        }
    }
}
