using EtherscanTest.Interfaces;
using Serilog;
using System;

namespace EtherscanTest.Implementations
{
    public class AppLogger : IAppLogger
    {
        private readonly ILogger logger;

        public AppLogger()
        {
            logger = Log.Logger;
        }

        public void LogInformation(string message)
        {
            logger.Information(message);
        }

        public void LogWarning(string message)
        {
            logger.Warning(message);
        }

        public void LogError(Exception exception, string message)
        {
            logger.Error(exception, message);
        }
    }
}
