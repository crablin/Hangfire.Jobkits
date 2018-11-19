namespace Hangfire.JobKits
{
    internal struct JobKitRoute
    {
        internal struct Standby
        {
            internal const string Url = "/standby";
            internal const string CategoryUrl = "/standby/(?<categoryId>.+)";
            internal const string LaunchUrl = "/standby-launch/job";
            internal const string LaunchRecurringUrl = "/standby-launch/recurring";
            internal const string JsUrl = "/standby-content/js";
            internal const string CssUrl = "/standby-content/css";
        }
        
    }
}
