using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Cardlytics.BasicApi.Logging
{
    public static class LoggerExtensions
    {
        public static void LogObject(this ILogger logger, LogLevel level, object logObject)
        {
            var logEntry = JsonConvert.SerializeObject(logObject);
            logger.Log(level, logEntry);
        }
    }
}
