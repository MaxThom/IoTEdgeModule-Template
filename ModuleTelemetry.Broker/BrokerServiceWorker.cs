using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ModuleTelemetry.Broker.configurations;
using ModuleTelemetry.Core;
using ModuleTelemetry.IotHub.abstracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModuleTelemetry.Broker
{
    public class BrokerServiceWorker : IHostedService
    {
        private readonly ILogger<BrokerServiceWorker> _logger;
        private readonly IOptions<BrokerOptions> _brokerOptions;
        private ITemporisingService _temporisingService;

        public BrokerServiceWorker(ILogger<BrokerServiceWorker> logger, IOptions<BrokerOptions> brokerOptions, ITemporisingService temporisingService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _brokerOptions = brokerOptions ?? throw new ArgumentNullException(nameof(brokerOptions));
            _temporisingService = temporisingService ?? throw new ArgumentNullException(nameof(temporisingService));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Initializing Broker data...");

            Run(cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Closing Broker data...");

            return Task.CompletedTask;
        }

        private async void Run(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var data = GenerateFakeData();
                _logger.LogInformation($"Generated {data.Count} datapoints");
                _temporisingService.StoreTelemetry(data.ToArray());

                await Task.Delay(_brokerOptions.Value.TimeBetweenGeneration, token).ConfigureAwait(false);
            }
        }

        private List<Telemetry> GenerateFakeData()
        {
            var telemetry = new List<Telemetry>();
            var random = new Random();
            var nb = random.Next(0, _brokerOptions.Value.MaxNumberOfDataGenerated);

            for (int i = 0; i < nb; i++)
            {
                telemetry.Add(new Telemetry()
                {
                    Name = "Temperature",
                    Value = random.Next(_brokerOptions.Value.DataMinValue, _brokerOptions.Value.DataMaxValue)
                });
            }

            return telemetry;
        }

        public static int Square(int x) => x * x;
    }
}