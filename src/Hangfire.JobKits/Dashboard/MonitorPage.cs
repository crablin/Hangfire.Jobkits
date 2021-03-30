using Hangfire.JobKits.Worker;

namespace Hangfire.JobKits.Dashboard
{
    internal partial class MonitorPage
    {
        /// <summary>
        /// Selected category.
        /// </summary>
        public string SelectedCategory { get; }
        /// <summary>
        /// Standby job map.
        /// </summary>
        public MonitorMap Map { get; }
        /// <summary>
        /// Standby options.
        /// </summary>
        public JobKitOptions Options { get; }

        public MonitorPage(string selectedCategory, MonitorMap map, JobKitOptions options)
        {
            SelectedCategory = selectedCategory;
            Map = map;
            Options = options;
        }

    }
}
