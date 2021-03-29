using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hangfire.JobKits.Worker
{
    internal class MonitorMap
    {
        public Dictionary<string, int> JobCategories { get; }
        public Dictionary<string, MonitorJob> JobCollection { get; }

        public MonitorMap(Dictionary<string, MonitorJob> collection)
        {
            JobCollection = collection;

            var total = collection.Select(o => o.Value).ToList();

            JobCategories = new Dictionary<string, int>();
            JobCategories.Add("全部", total.Count());
            foreach (var category in collection.Values.GroupBy(x => x.CategoryName).OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Count()))
            {
                JobCategories.Add(category.Key, category.Value);
            }
        }
    }
}
