using ModuleTelemetry.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleTelemetry.IotHub.abstracts
{
    public interface ITemporisingService
    {
        List<Telemetry> GetTelemetryToWrite();

        void StoreTelemetry(params Telemetry[] dataPoints);
    }
}