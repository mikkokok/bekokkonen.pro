using bekokkonen.pro.Global.Enum;

namespace bekokkonen.pro.Global.Config
{
    public static class GlobalConfig
    {
        public static ApiDocument? ApiDocumentConfig { get; set; }
        public static RabbitMQ? RabbitMQConfig { get; set; }
        public static HeatHarmony? HeatHarmonyConfig { get; set; }
        public static PowerPricing? PowerPricingConfig { get; set; }
        public static Shelly1PM? Shelly1PMConfig { get; set; }
        public static RestlessFalcon? RestlessFalconConfig { get; set; }

        public class ApiDocument
        {
            public required string Title { get; set; }
            public required string Version { get; set; }
        }

        public class RabbitMQ
        {
            public required string mqttServer { get; set; }
            public required string mqttUser { get; set; }
            public required string mqttPassword { get; set; }
            public required string mqttTopic { get; set; }
        }

        public class HeatHarmony
        {
            public required string ApiKey { get; set; }
            public required string BaseUrl { get; set; }
        }

        public class PowerPricing
        {
            public required double TransferFee { get; set; }
            public required double EnergyTax1 { get; set; }
            public required double EnergyTax2 { get; set; }
            public required double Margin { get; set; }
            public required double YieldFee { get; set; }
        }

        public class Shelly1PM
        {
            public required Dictionary<Shelly1PMDeviceName, Shelly1PMDevice> Devices { get; set; } = [];
        }

        public class Shelly1PMDevice
        {
            [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
            public Shelly1PMDeviceName Name { get; set; }
            public required string IPAddress { get; set; }
            public bool RelayStatus { get; set; } = false;
        }

        public class RestlessFalcon
        {
            public required string Url { get; set; }
            public required string SslThumbprint { get; set; }
        }
    }
}
