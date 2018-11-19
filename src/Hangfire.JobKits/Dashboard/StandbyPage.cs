using Hangfire.JobKits.Worker;

namespace Hangfire.JobKits.Dashboard
{
    internal partial class StandbyPage
    {
        /// <summary>
        /// Selected category.
        /// </summary>
        public string SelectedCategory { get; }
        /// <summary>
        /// Standby job map.
        /// </summary>
        public StandbyMap Map { get; }
        /// <summary>
        /// Standby options.
        /// </summary>
        public JobKitOptions Options { get; }

        public StandbyPage(string selectedCategory, StandbyMap map, JobKitOptions options)
        {
            SelectedCategory = selectedCategory;
            Map = map;
            Options = options;
        }
    }
}
