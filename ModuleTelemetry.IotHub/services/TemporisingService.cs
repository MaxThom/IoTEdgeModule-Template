using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ModuleTelemetry.Core;
using ModuleTelemetry.IotHub.abstracts;

namespace ModuleTelemetry.IotHub.services
{
    public class TemporisingService : ITemporisingService
    {
        private List<Telemetry> _telemetryStore = new List<Telemetry>();
        private readonly object _telemetryStoreLocker = new object();

        public List<Telemetry> GetTelemetryToWrite()
        {
            lock (_telemetryStoreLocker)
            {
                var telemetry = new List<Telemetry>(_telemetryStore);
                _telemetryStore.Clear();
                return telemetry;
            }
        }

        public void StoreTelemetry(params Telemetry[] dataPoints)
        {
            lock (_telemetryStoreLocker)
            {
                _telemetryStore.AddRange(dataPoints);
            }
        }
    }
}