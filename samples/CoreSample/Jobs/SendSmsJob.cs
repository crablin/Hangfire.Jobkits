using Hangfire.Console;
using Hangfire.JobKits;
using Hangfire.Server;
using CoreSample.Jobs.DI;
using Hangfire;
using System;

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
        //[JobValidation(StartHour = 8, StartMinute = 10, EndHour = 20, EndMinute = 10)]
        [SendSmsJobValidation(Name = "SendSms", Cron = "30 0 * * *", Range = ValidateRangeType.Daily)]
        [JobMethod(RecurringJobId = "SendSms", Name = "發送簡訊", Description = "說明：寄送一筆簡訊資訊")]
        public bool Send(PerformContext context, [JobParam(Description = "簡訊文字")] string text)
        {
            context.WriteLine("OK");
            context.WriteLine(_sendProgress.Send(text));
            context.WriteLine("OK");
            return false;
        }

        //[LogEverything]
        //[SendSmsJobValidation]
        [SendSmsJobValidation(Name = "SendSmsError", Cron = "0 0 1 * *", Range = ValidateRangeType.Monthly)]
        [AutomaticRetry(Attempts = 2, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [JobMethod(RecurringJobId = "SendErrorSms", Name = "發送失敗簡訊", Description = "說明：寄送一筆失敗簡訊資訊")]
        public bool SendError(PerformContext context, [JobParam(Description = "簡訊文字")] string text)
        {
            context.WriteLine("OK");
            context.WriteLine("OK");

            context.WriteLine();
            return true;
        }

        private Func<bool> Validate = () =>
        {
            return false;
        };


    }
}
