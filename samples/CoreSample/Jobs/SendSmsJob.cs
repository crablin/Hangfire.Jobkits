using System;
using CoreSample.Jobs.DI;
using CoreSample.Jobs.Validation;
using Hangfire;
using Hangfire.Console;
using Hangfire.JobKits;
using Hangfire.Server;

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

        [JobValidation(Name = "SendSms", Cron = "30 0 * * *", Range = ValidateRangeType.Daily)]
        [JobMethod(RecurringJobId = "SendSms", Name = "發送簡訊", Description = "說明：寄送一筆簡訊資訊")]
        public bool Send(PerformContext context, [JobParam(Description = "簡訊文字")] string text)
        {
            context.WriteLine("Start");
            context.WriteLine(_sendProgress.Send(text));
            context.WriteLine("End");
            context.WriteLine();

            return false;
        }

        [SendSmsJobValidation(Name = "SendSmsError", Cron = "0 0 1 * *", Range = ValidateRangeType.Monthly)]
        [AutomaticRetry(Attempts = 2, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [JobMethod(RecurringJobId = "SendErrorSms", Name = "發送失敗簡訊", Description = "說明：寄送一筆失敗簡訊資訊")]
        public bool SendError(PerformContext context, [JobParam(Description = "簡訊文字")] string text)
        {
            context.WriteLine("Start");
            context.WriteLine("End");
            context.WriteLine();
            return true;
        }

        public void AddValidation(PerformContext context, string action, DateTime date)
        {
            //JobStorage.Current.GetConnection().GetAllEntriesFromHash()
           
        }

    }
}