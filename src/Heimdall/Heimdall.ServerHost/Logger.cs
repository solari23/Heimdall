using System;

namespace Heimdall.ServerHost
{
    public class Logger : Heimdall.Server.IHeimdallLogger
    {
        public void LogError(string message)
        {
            Console.Error.WriteLine(message);
        }

        public void LogInformation(string message)
        {
            Console.WriteLine(message);
        }

        public void LogPerRequestInformation(string requestDetails)
        {
            LogInformation(requestDetails);
        }

        public void LogWarning(string message)
        {
            Console.WriteLine(message);
        }
    }
}
