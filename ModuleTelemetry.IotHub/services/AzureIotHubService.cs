using DotNetty.Transport.Channels;

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Logging;

using ModuleTelemetry.Core;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ModuleTelemetry.IotHub.services
{
    public class AzureIotHubService
    {
        private ModuleClient _iotHubModuleClient { get; set; }

        private ILogger<AzureIotHubService> _logger { get; set; }

        public AzureIotHubService(ILogger<AzureIotHubService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #region Connection

        public async Task OpenIotHubConnection()
        {
            // Open a connection to the Edge runtime
            _iotHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync();
            await _iotHubModuleClient.SetMethodDefaultHandlerAsync(OnDefaultDirectMethodInvoke, this);
            await _iotHubModuleClient.OpenAsync();
            _logger.LogInformation("IoT Hub module client initialized.");
        }

        public async Task CloseIotHubConnection()
        {
            // Close a connection to the Edge runtime
            await _iotHubModuleClient.CloseAsync();
            _iotHubModuleClient?.Dispose();
            _logger.LogInformation("IoT Hub module client closed.");
        }

        private async Task ResetIotHubConnection()
        {
            _iotHubModuleClient?.Dispose();
            await OpenIotHubConnection();
            _logger.LogInformation("IoT Hub module client connection reset.");
        }

        #endregion Connection

        #region ModuleTwins

        public async Task<Twin> GetModuleTwin()
             => await _iotHubModuleClient.GetTwinAsync().ConfigureAwait(false);

        public async Task UpdateReportedPropertiesModuleTwin(TwinCollection reportedProperties)
            => await _iotHubModuleClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);

        public async Task AddOnDesiredPropertyChangedHandlerModuleTwin(DesiredPropertyUpdateCallback onDesiredPropertyChangedHandler, object userContext)
            => await _iotHubModuleClient.SetDesiredPropertyUpdateCallbackAsync(onDesiredPropertyChangedHandler, userContext).ConfigureAwait(false);

        #endregion ModuleTwins

        #region Message

        private async Task SendMessageByChunk<T>(int chunkSize, List<T> dataSet, Func<List<T>, Message> messageGenerationProcedure)
        {
            for (int i = 0; i < dataSet.Count; i += chunkSize)
            {
                var items = dataSet.Skip(i).Take(chunkSize).ToList();
                var message = messageGenerationProcedure(items);
                try
                {
                    await _iotHubModuleClient.SendEventAsync(message);
                }
                catch (ObjectDisposedException)
                {
                    await ResetIotHubConnection();
                    await _iotHubModuleClient.SendEventAsync(message);
                }
                catch (ClosedChannelException)
                {
                    await ResetIotHubConnection();
                    await _iotHubModuleClient.SendEventAsync(message);
                }

                await Task.Delay(100);
            }
        }

        public async Task SendTelemetry(List<Telemetry> telemetryList)
        {
            await SendMessageByChunk(20, telemetryList, (chunk) =>
            {
                var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(chunk)))
                {
                    ContentType = "application/json",
                    ContentEncoding = "charset=utf-8"
                };
                message.Properties["type"] = "telemetry";
                return message;
            });
        }

        #endregion Message

        private async Task<MethodResponse> OnDefaultDirectMethodInvoke(MethodRequest methodRequest, object userContext)
        {
            _logger.LogInformation("Received message on default method invoke: {0}", Encoding.ASCII.GetString(methodRequest.Data));

            return new MethodResponse(Encoding.ASCII.GetBytes($"{{\"result\": \"This direct method invoke as no handler: {methodRequest.Name}\"}}"), (int)HttpStatusCode.OK);
        }
    }
}