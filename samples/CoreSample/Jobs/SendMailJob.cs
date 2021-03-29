using Hangfire.Console;
using Hangfire.JobKits;
using Hangfire.Server;
using CoreSample.Jobs.DI;
using System.Collections.Generic;
using System;

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
        [JobValidation(StartHour = 8, StartMinute = 10, EndHour = 20, EndMinute = 10)]
        [JobMethod(UseQueue = true, RecurringJobId = "SendMail_E01", Name = "SendMail_E01")]
        public bool Send(PerformContext context)
        {
            context.WriteLine("OK");
            context.WriteLine(_sendProgress.Send("Crab Good!!"));
            context.WriteLine("OK");

            context.WriteLine();
            return true;

        }
        [JobValidation(StartHour = 8, StartMinute = 10, EndHour = 20, EndMinute = 10)]
        [JobMethod(Name = "Send Model", RecurringJobId = "SendModel_E01")]
        public bool Send(PerformContext context, MailModel model)
        {
            context.WriteLine($"Subject: {model.Subject}");
            context.WriteLine($"Body: {model.Body}");
            context.WriteLine("OK");
            return true;

        }
        [JobValidation(StartHour = 8, StartMinute = 10, EndHour = 20, EndMinute = 10)]
        [JobMethod(Name = "xxxxxxxx Model", RecurringJobId = "xxxxxxxx")]
        public bool xxxxxxxx(PerformContext context)
        {
            Console.WriteLine("1");
            return false;

        }
        [JobValidation(StartHour = 8, StartMinute = 10, EndHour = 20, EndMinute = 10)]
        [JobMethod(Name = "Send List", RecurringJobId = "SendDictionary_E01")]
        public bool SendList(PerformContext context, List<MailModel> list, Status status)
        {
            context.WriteLine($"Count: {list.Count}");
            context.WriteLine($"Status: {status}");
            context.WriteLine("OK");
            return true;

        }
        [JobValidation(StartHour = 8, StartMinute = 10, EndHour = 20, EndMinute = 10)]
        [JobMethod(Name = "Send Dictionary", RecurringJobId = "Send Dictionary")]
        public bool SendDictionary(PerformContext context, Dictionary<int, MailModel> list, Status status)
        {
            context.WriteLine($"Count: {list.Count}");
            context.WriteLine($"Status: {status}");
            context.WriteLine("OK");
            return false;
        }

        [JobValidation(StartHour = 8, StartMinute = 10, EndHour = 20, EndMinute = 10)]
        [JobMethod(Name = "Send List String and Int", RecurringJobId = "SendList String and Int_E01")]
        public bool SendDictionary(PerformContext context, List<int> list, List<string> names)
        {
            context.WriteLine($"Count: {list.Count}");
            context.WriteLine($"Names: {names.Count}");
            context.WriteLine("OK");
            return true;

        }
    }

    public class MailModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public DateTime SentTime { get; set; }
    }

    public enum Status
    {
        Yes, No
    }

}
