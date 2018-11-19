using Hangfire.Console;
using Hangfire.JobKits;
using Hangfire.Server;
using CoreSample.Jobs.DI;

namespace CoreSample.Jobs
{

    [JobLauncher(CategoryName = "電子郵件")]
    public class SendMailJob2
    {
        private ISendProgress _sendProgress;

        public SendMailJob2(ISendProgress sendProgress)
        {
            _sendProgress = sendProgress;
        }

        [JobMethod]
        public void Send(PerformContext context)
        {
            context.WriteLine("OK");
            context.WriteLine(_sendProgress.Send("Crab Good!!"));
            context.WriteLine("OK");

            context.WriteLine();
        }
    }
}
