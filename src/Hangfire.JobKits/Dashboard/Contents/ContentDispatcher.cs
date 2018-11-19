using System;
using System.Reflection;
using System.Threading.Tasks;
using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Hangfire.JobKits.Dashboard.Contents
{
    internal class ContentDispatcher : IDashboardDispatcher
    {
        private Assembly _currentAssembly = typeof(ContentDispatcher).Assembly;

        private string _contentType;
        private string _resourceName;
        private TimeSpan _expiredTime;

        public ContentDispatcher(string contentType, string resourceName, TimeSpan expiredTime)
        {
            _contentType = contentType;
            _resourceName = resourceName;
            _expiredTime = expiredTime;
        }

        public async Task Dispatch([NotNull] DashboardContext context)
        {
            context.Response.ContentType = _contentType;
            context.Response.SetExpire(DateTimeOffset.UtcNow + _expiredTime);
            await WriteResourceAsync(context);
        }

        private async Task WriteResourceAsync(DashboardContext context)
        {
            using (var stream = _currentAssembly.GetManifestResourceStream(_resourceName))
            {
                if (stream == null)
                {
                    context.Response.StatusCode = 404;
                }
                else
                {
                    await stream.CopyToAsync(context.Response.Body);
                }
            }
        }
    }
}
