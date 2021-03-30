using System.Collections.Generic;
using System.Linq;
using Hangfire.JobKits.Resources;

namespace Hangfire.JobKits.Worker
{
    internal class MonitorMap
    {
        public Dictionary<string, int> JobCategories { get; }
        public Dictionary<string, List<MonitorJob>> JobCollection { get; }

        public MonitorMap(Dictionary<string, List<MonitorJob>> collection)
        {
            JobCollection = collection;

            JobCategories = new Dictionary<string, int>
            {
                { ValidateRangeType.Daily, TryGetJobCount(collection, ValidateRangeType.Daily) },
                { ValidateRangeType.Weekly, TryGetJobCount(collection, ValidateRangeType.Weekly) },
                { ValidateRangeType.Monthly, TryGetJobCount(collection, ValidateRangeType.Monthly) }
            };
        }

        protected static int TryGetJobCount(Dictionary<string, List<MonitorJob>> collection, string rangeKey)
        {
            return collection.ContainsKey(rangeKey) ? collection[rangeKey].Count() : 0;
        }
    }
}