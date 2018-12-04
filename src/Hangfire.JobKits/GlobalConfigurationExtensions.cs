using System;
using System.Linq;
using System.Reflection;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Hangfire.JobKits.Dashboard;
using Hangfire.JobKits.Dashboard.Contents;
using Hangfire.JobKits.Resources;
using Hangfire.JobKits.Worker;

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
    }
}