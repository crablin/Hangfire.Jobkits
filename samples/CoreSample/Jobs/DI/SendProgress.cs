using System;

namespace CoreSample.Jobs.DI
{
    public class SendProgress : ISendProgress
    {
        public string Send(string text)
        {
            return $"{text} - {DateTime.Now}";
        }
    }
}
