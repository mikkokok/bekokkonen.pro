namespace bekokkonen.pro.Global.Config
{
    public static class GlobalConfig
    {
        public static ApiDocument? ApiDocumentConfig { get; set; }
        public static RabbitMQ? RabbitMQConfig { get; set; }
        public static HeatHarmony? HeatHarmonyConfig { get; set; }

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
    }
}
