using System;
using System.Linq;
using System.Reflection;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Hangfire.JobKits.Dashboard;
using Hangfire.JobKits.Dashboard.Contents;
using Hangfire.JobKits.Providers;
using Hangfire.JobKits.Resources;
using Hangfire.JobKits.Worker;
using Microsoft.AspNetCore.Builder;

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
            this IGlobalConfiguration configuration, params Assembly[] assemblies)
            => configuration.UseJobMonitor(new JobKitOptions
            {
                RequireConfirmation = true
            }, assemblies);

        [PublicAPI]
        public static IGlobalConfiguration UseJobMonitor(
            this IGlobalConfiguration configuration, JobKitOptions options, params Assembly[] assemblies)
        {
            var map = MonitorHelper.GetMap(assemblies);
            
            if (map != null)
            {
                DashboardRoutes.Routes.AddRazorPage(JobKitRoute.Monitor.Url, x => new MonitorPage(ValidateRangeType.Daily, map, options));
                DashboardRoutes.Routes.AddRazorPage(JobKitRoute.Monitor.CategoryUrl, x => new MonitorPage(x.Groups["categoryId"].Value, map, options));

                NavigationMenu.Items.Add(page => new MenuItem(Strings.MonitorPage_Title, page.Url.To(JobKitRoute.Monitor.Url))
                {
                    Active = page.RequestPath.StartsWith(JobKitRoute.Monitor.Url),
                    Metric = new DashboardMetric("monitor-count", x => new Metric(map.JobCollection.Values.Sum(y => y.Count)))
                });
                
            }
            return configuration;
        }
        
        [PublicAPI]
        public static IApplicationBuilder UseHangfireMonitor(
            [NotNull] this IApplicationBuilder app)
        {
            Common.JobFilterProviders.Providers.Add(new TypeJobFilterProvider(app.ApplicationServices));
            
            return app;
        }
    }
}