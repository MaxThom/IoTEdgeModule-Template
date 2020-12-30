using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ModuleTelemetry.IotHub.abstracts;
using ModuleTelemetry.IotHub.configurations;
using ModuleTelemetry.IotHub.services;

using Newtonsoft.Json;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ModuleTelemetry.IotHub
{
    public class AzureIotHubServiceWorker : IHostedService
    {
        private readonly ILogger<AzureIotHubServiceWorker> _logger;
        private readonly IOptions<IotHubOptions> _iotHubOptions;
        private AzureIotHubService _azureIotHubService;
        private ITemporisingService _temporisingService;

        public AzureIotHubServiceWorker(ILogger<AzureIotHubServiceWorker> logger, IOptions<IotHubOptions> iotHubOptions
                                      , AzureIotHubService azureIotHubService, ITemporisingService temporisingService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _iotHubOptions = iotHubOptions ?? throw new ArgumentNullException(nameof(iotHubOptions));
            _azureIotHubService = azureIotHubService ?? throw new ArgumentNullException(nameof(azureIotHubService));
            _temporisingService = temporisingService ?? throw new ArgumentNullException(nameof(temporisingService));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Initializing IotHub connection for {_iotHubOptions.Value.ModuleName} ...");
            await _azureIotHubService.OpenIotHubConnection();

            await _azureIotHubService.AddOnDesiredPropertyChangedHandlerModuleTwin(OnModuleTwinDesiredPropertiesUpdated, nameof(AzureIotHubServiceWorker));

            Twin twin = await _azureIotHubService.GetModuleTwin();
            _logger.LogInformation(twin.ToJson(Formatting.Indented));

            Run(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Closing IotHub connection ...");
            _azureIotHubService.CloseIotHubConnection().ConfigureAwait(false);

            return Task.CompletedTask;
        }

        private async void Run(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(_iotHubOptions.Value.TimeBetweenUpload, token).ConfigureAwait(false);

                var telemetryToWrite = _temporisingService.GetTelemetryToWrite();
                if (telemetryToWrite.Any())
                {
                    _logger.LogInformation($"Uploading {telemetryToWrite.Count} telemetry points");
                    await _azureIotHubService.SendTelemetry(telemetryToWrite).ConfigureAwait(false);
                }
            }
        }

        public async Task OnModuleTwinDesiredPropertiesUpdated(TwinCollection desiredProperties, object userContext)
        {
            // Receive all desired properties
            _logger.LogInformation(desiredProperties.ToJson(Formatting.Indented));

            //await _azureIotHubService.UpdateReportedPropertiesModuleTwin(desiredProperties);
        }

        public static int Square(int x) => x * x;
    }
}