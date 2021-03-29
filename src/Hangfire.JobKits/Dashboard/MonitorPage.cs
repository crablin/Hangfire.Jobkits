using Hangfire.Common;
using Hangfire.JobKits.Worker;
using Hangfire.Storage.Monitoring;
using System;
using System.Collections.Generic;
using System.Linq;

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
