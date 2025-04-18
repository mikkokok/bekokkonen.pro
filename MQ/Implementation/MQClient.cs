using bekokkonen.pro.Config;
using MQTTnet;
using MQTTnet.Packets;
using System.Text;
using System.Text.Json;

namespace bekokkonen.pro.MQ.Implementation
{
    public class MQClient
    {
        private string _serviceName;
        private readonly string _clientId = "bekokkonenpro";
        private ILogger<MQClient> _logger;
        private IConfiguration _config;
        private GlobalConfig.RabbitMQ _mqConfig;

        public MQClient(ILogger<MQClient> logger, IConfiguration config)
        {
            _serviceName = nameof(MQClient);
            _logger = logger;
            _config = config;
            _mqConfig = GlobalConfig.RabbitMQConfig!;
            Task mqTask = StartMqttClient();
        }

        private async Task StartMqttClient()
        {
            _logger.LogInformation($"{_serviceName}:: Start MQtt client");
            var mqttClient = new MqttClientFactory().CreateMqttClient();
            mqttClient.ApplicationMessageReceivedAsync += m => HandleMessage(m.ApplicationMessage);
            var topicFilter = new MqttTopicFilter
            {
                Topic = _mqConfig.mqttTopic,
            };
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(_mqConfig.mqttServer, 1883)
                .WithClientId(_clientId)
                .WithCredentials(_mqConfig.mqttUser, _mqConfig.mqttPassword)
                .WithCleanSession()
                .Build();
            var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            await mqttClient.SubscribeAsync(topicFilter);
            _logger.LogInformation($"{_serviceName}:: MQtt client connected successfully");
        }

        private Task HandleMessage(MqttApplicationMessage applicationMessage)
        {
            var payload = Encoding.UTF8.GetString(applicationMessage.Payload);
            _logger.LogInformation($"{_serviceName}:: Received message {payload}");
            //var deSerializerOptions = new JsonSerializerOptions();
            //deSerializerOptions.Converters.Add(new DoubleFromJsonConverter());
            //var data = JsonSerializer.Deserialize<SensorData>(payload, deSerializerOptions);
            return Task.CompletedTask;
        }
    }
}
