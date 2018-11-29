using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Cardlytics.BasicApi.Logging;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;

namespace Cardlytics.BasicApi.Middleware
{
    public class RequestLogMiddleware
    {
        private readonly RequestDelegate next;

        private readonly ILogger logger;

        public RequestLogMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            logger = loggerFactory.CreateLogger("RequestLogger");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;
            request.EnableRewind();

            var logEntry = new RequestLogEntry
            {
                Url = request.Path,
                Method = request.Method,
                Timestamp = DateTime.UtcNow
            };

            // default values and do not dispose underlying stream
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                logEntry.Body = await reader.ReadToEndAsync();
            }

            request.Body.Position = 0;
            var timer = Stopwatch.StartNew();

            await next(context);

            logEntry.DurationMs = timer.ElapsedMilliseconds;
            logEntry.StatusCode = context.Response.StatusCode;

            logger.LogObject(LogLevel.Information, logEntry);
        }
    }
}
