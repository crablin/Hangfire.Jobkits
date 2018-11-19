using Hangfire.Annotations;

namespace Hangfire.JobKits
{
    [PublicAPI]
    public sealed class JobKitOptions
    {
        public bool RequireConfirmation { get; set; }

        public bool AlwaysCollapsed { get; set; }
    }
}
