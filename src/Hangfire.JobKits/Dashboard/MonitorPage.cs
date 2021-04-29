using Hangfire.JobKits.Resources;
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

        public string SelectedCategoryText
        {
            get
            {
                switch (this.SelectedCategory)
                {
                    case ValidateRangeType.Daily: return Strings.Range_Daily;
                    case ValidateRangeType.Weekly: return Strings.Range_Weekly;
                    case ValidateRangeType.Monthly: return Strings.Range_Monthly;
                    default: return string.Empty;
                }
            }
        }

        public MonitorPage(string selectedCategory, MonitorMap map, JobKitOptions options)
        {
            SelectedCategory = selectedCategory;
            Map = map;
            Options = options;
        }
    }
}