using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Hangfire.JobKits.Dashboard;
using Hangfire.JobKits.Dashboard.Contents;
using Hangfire.JobKits.DataBase;
using Hangfire.JobKits.Resources;
using Hangfire.JobKits.Worker;
using Hangfire.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Hangfire.JobKits
{
    public static class GlobalConfigurationExtensions
    {
        [PublicAPI]
        public static IGlobalConfiguration UseJobKits(
            this IGlobalConfiguration configuration, params Assembly[] assemblies)
            => configuration.UseJobKits(new JobKitOptions
            {
                RequireConfirmation = true
            }, assemblies);

        [PublicAPI]
        public static IGlobalConfiguration UseJobMonitor(
    this IGlobalConfiguration configuration, params Assembly[] assemblies)
    => configuration.UseJobMonitor(new JobKitOptions
    {
        RequireConfirmation = true
    }, assemblies);

        [PublicAPI]
        public static IGlobalConfiguration UseJobKits(
            this IGlobalConfiguration configuration, JobKitOptions options, params Assembly[] assemblies)
        {
            var map = StandbyHelper.GetMap(assemblies);

            if (map != null)
            {
                DashboardRoutes.Routes.AddRazorPage(JobKitRoute.Standby.Url, x => new StandbyPage(map.JobCategories.First().Key, map, options));
                DashboardRoutes.Routes.AddRazorPage(JobKitRoute.Standby.CategoryUrl, x => new StandbyPage(x.Groups["categoryId"].Value, map, options));
                DashboardRoutes.Routes.Add(JobKitRoute.Standby.LaunchUrl, new BackgroundJobDispatcher(map));
                DashboardRoutes.Routes.Add(JobKitRoute.Standby.LaunchRecurringUrl, new RecurringJobDispatcher(map, options));

                NavigationMenu.Items.Add(page => new MenuItem(Strings.Standby_Title, page.Url.To(JobKitRoute.Standby.Url))
                {
                    Active = page.RequestPath.StartsWith(JobKitRoute.Standby.Url),
                    Metric = new DashboardMetric("standby-count", x => new Metric(map.JobCollection.Count))
                });

                DashboardRoutes.Routes.Add(
                    JobKitRoute.Standby.JsUrl,
                    new ContentDispatcher("application/js", "Hangfire.JobKits.Dashboard.Contents.standby.js", TimeSpan.FromDays(1)));

                DashboardRoutes.Routes.Add(
                    JobKitRoute.Standby.CssUrl,
                    new ContentDispatcher("text/css", "Hangfire.JobKits.Dashboard.Contents.standby.css", TimeSpan.FromDays(1)));
            }
            return configuration;
        }
        [PublicAPI]
        public static IGlobalConfiguration UseJobMonitor(
      this IGlobalConfiguration configuration, JobKitOptions options, params Assembly[] assemblies)
        {
            var map = MonitorHelper.GetMap(assemblies);
            configuration.UseFilter<JobValidationAttribute>(new JobValidationAttribute());
            if (map != null)
            {
                DashboardRoutes.Routes.AddRazorPage(JobKitRoute.Monitor.Url, x => new MonitorPage("全部", map, options));
                DashboardRoutes.Routes.AddRazorPage(JobKitRoute.Monitor.CategoryUrl, x => new MonitorPage(x.Groups["categoryId"].Value, map, options));
                NavigationMenu.Items.Add(page => new MenuItem("job-Monitor", page.Url.To(JobKitRoute.Monitor.Url))
                {
                    Active = page.RequestPath.StartsWith(JobKitRoute.Monitor.Url),
                    Metric = new DashboardMetric("monitor-count", x => new Metric(map.JobCollection.Count))
                });
            }
            return configuration;
        }
        public static IApplicationBuilder UseValidation<TValidation>(
         [NotNull] this IApplicationBuilder builder, [NotNull] TValidation validation)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (validation == null) throw new ArgumentNullException(nameof(validation));

            return builder.UseValidation<TValidation>(validation, x => GlobalJobValidations.Validation = x as IJobValidation);
        }
        public static IApplicationBuilder UseValidation<T>(
            [NotNull] this IApplicationBuilder builder, T validation,
            [NotNull] Action<T> validationAction)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            validationAction(validation);

            return builder;
        }
        public static IServiceCollection AddJobMonitor(this IServiceCollection service)
        {
            service.AddHostedService<BackGroundJob>();
            return service;
        }
        [PublicAPI]
        public static IApplicationBuilder UseHangfireMoitor(
            [NotNull] this IApplicationBuilder app, params Assembly[] assemblies
            )
        {

            if (app == null) throw new ArgumentNullException(nameof(app));

            var services = app.ApplicationServices;

            var storage = services.GetRequiredService<JobStorage>();

            var jobLauncherTypes = assemblies
             .SelectMany(x => x.GetTypes())
             .Where(x => x.GetCustomAttribute<JobLauncherAttribute>() != null);

            var methods = jobLauncherTypes.GetMonitorMethods();

            using (var connection = storage.GetConnection())
            {
                var storageConnection = connection as JobStorageConnection;
                if (storageConnection != null)
                {

                    Parallel.ForEach(methods.GetMonitor(), job =>
                    {
                        Console.WriteLine(job.Key);
                        storageConnection.SetRangeInHash($"recurring-Monitor:{job.Key.ToString()}", job.Value);
                        storageConnection.SetRangeInHash("Key", new Dictionary<string, string> { { $"RecurringJobId_{ job.Key }", job.Key } });

                    });
                }
            }
            return app;
        }
    }
}