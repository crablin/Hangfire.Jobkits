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

        [JobMethod]
        public void Send(PerformContext context)
        {
            context.WriteLine("OK");
            context.WriteLine(_sendProgress.Send("Crab Good!!"));
            context.WriteLine("OK");

            context.WriteLine();
        }

        [JobMethod(Name = "Send Model")]
        public void Send(PerformContext context, MailModel model)
        {
            context.WriteLine($"Subject: {model.Subject}");
            context.WriteLine($"Body: {model.Body}");
            context.WriteLine("OK");
        }

        [JobMethod(Name = "Send List")]
        public void SendList(PerformContext context, List<MailModel> list, Status status)
        {
            context.WriteLine($"Count: {list.Count}");
            context.WriteLine($"Status: {status}");
            context.WriteLine("OK");
        }

        [JobMethod(Name = "Send Dictionary")]
        public void SendDictionary(PerformContext context, Dictionary<int, MailModel> list, Status status)
        {
            context.WriteLine($"Count: {list.Count}");
            context.WriteLine($"Status: {status}");
            context.WriteLine("OK");
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
