using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtherscanTest.Interfaces
{
    public interface IAppLogger
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(Exception exception, string message);
    }
}
