namespace ModuleTelemetry.Broker.configurations
{
    public class BrokerOptions
    {
        public int TimeBetweenGeneration { get; set; }

        public int MaxNumberOfDataGenerated { get; set; }

        public int DataMinValue { get; set; }

        public int DataMaxValue { get; set; }
    }
}