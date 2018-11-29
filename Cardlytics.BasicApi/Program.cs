using System;
using System.Reflection;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

using NLog;
using NLog.Web;

using StructureMap.AspNetCore;

using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Cardlytics.BasicApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            NLogBuilder.ConfigureNLog("nlog.config");

            var programLogger = LogManager.GetCurrentClassLogger();
            var serviceName = Assembly.GetExecutingAssembly().GetName();

            try
            {
                programLogger.Info($"Starting service {serviceName}");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                programLogger.Error(ex, $"Starting service {serviceName}");
            }
            finally
            {
                programLogger.Info($"Shutting down service {serviceName}");
                LogManager.Shutdown();
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost
                .CreateDefaultBuilder(args)
                .UseIISIntegration()
                .UseStartup<Startup>()
                .ConfigureLogging(log =>
                {
                    log.ClearProviders();
                    log.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
                .UseStructureMap();
    }
}
