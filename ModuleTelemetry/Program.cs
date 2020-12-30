using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using ModuleTelemetry.Broker;
using ModuleTelemetry.Broker.configurations;
using ModuleTelemetry.configurations;
using ModuleTelemetry.IotHub;
using ModuleTelemetry.IotHub.abstracts;
using ModuleTelemetry.IotHub.configurations;
using ModuleTelemetry.IotHub.services;

using System;
using System.IO;

namespace ModuleTelemetry
{
    public class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).RunConsoleAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config.SetBasePath(Directory.GetCurrentDirectory());
                   config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                   config.AddEnvironmentVariables("MIR_");
                   config.AddCommandLine(args);
                   // {prefix}{parent}__{child}__{grandchild}
                   // ex: MIR_LOGGING__LOGLEVEL__DEFAULT
               })
               .ConfigureLogging((hostContext, logging) =>
               {
                   logging.ClearProviders();
                   logging.AddSimpleConsole((configure) =>
                   {
                       configure.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
                   });
               }).ConfigureServices((hostContext, services) =>
               {
                   services.AddOptions();
                   services.Configure<TelemetryOptions>(hostContext.Configuration.GetSection("Telemetry"));
                   services.Configure<IotHubOptions>(hostContext.Configuration.GetSection("IotHub"));
                   services.Configure<BrokerOptions>(hostContext.Configuration.GetSection("Broker"));

                   services.AddSingleton<ITemporisingService, TemporisingService>();
                   services.AddSingleton<AzureIotHubService>();

                   services.AddHostedService<AzureIotHubServiceWorker>();
                   services.AddHostedService<BrokerServiceWorker>();
               });

        public static int Square(int x) => x * x;
    }
}