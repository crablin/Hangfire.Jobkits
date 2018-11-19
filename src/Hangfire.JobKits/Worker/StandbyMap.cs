using System.Collections.Generic;
using System.Linq;

namespace Hangfire.JobKits.Worker
{
    internal class StandbyMap
    {
        public Dictionary<string, int> JobCategories { get; }
        public Dictionary<string, StandbyJob> JobCollection { get; }

        public StandbyMap(Dictionary<string, StandbyJob> collection)
        {
            JobCollection = collection;

            JobCategories = collection.Values
                .GroupBy(x => x.CategoryName)
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Count());
        }
    }
}
