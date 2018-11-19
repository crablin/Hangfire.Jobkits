using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreSample.Jobs.DI
{
    public interface ISendProgress
    {
        string Send(string text);
    }
}
