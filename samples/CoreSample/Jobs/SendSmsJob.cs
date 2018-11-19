using Hangfire.Console;
using Hangfire.JobKits;
using Hangfire.Server;
using CoreSample.Jobs.DI;

namespace CoreSample.Jobs
{

    [JobLauncher(CategoryName = "SendSms")]
    public class SendSmsJob
    {
        private ISendProgress _sendProgress;

        public SendSmsJob(ISendProgress sendProgress)
        {
            _sendProgress = sendProgress;
        }

        [JobMethod(RecurringJobId ="SendSms", Name = "發送簡訊")]
        public void Send(PerformContext context, [JobParam(Description = "簡訊文字")] string text)
        {
            context.WriteLine("OK");
            context.WriteLine(_sendProgress.Send(text));
            context.WriteLine("OK");

            context.WriteLine();
        }
    }
}
