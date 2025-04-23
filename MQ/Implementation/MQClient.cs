using bekokkonen.pro.Global.Config;
using bekokkonen.pro.Global.Interfaces;
using MQTTnet;
using MQTTnet.Packets;
using System.Text;
using System.Text.Json;

namespace bekokkonen.pro.MQ.Implementation
{
    public sealed class MQClient : IAsyncInitialization
    {
        private string _serviceName;
        private readonly string _clientId = "bekokkonenpro";
        private ILogger<MQClient> _logger;
        private GlobalConfig.RabbitMQ _mqConfig;

        public MQClient(ILogger<MQClient> logger)
        {
            _serviceName = nameof(MQClient);
            _logger = logger;
            _mqConfig = GlobalConfig.RabbitMQConfig!;
            Initialization = StartMqttClient();
        }

        public Task Initialization { get; private set; }

        private async Task StartMqttClient()
        {
            _logger.LogInformation($"{_serviceName}:: Start MQtt client");
            try
            {
                var mqttClient = new MqttClientFactory().CreateMqttClient();
                mqttClient.ApplicationMessageReceivedAsync += m => HandleMessage(m.ApplicationMessage);
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(_mqConfig.mqttServer, 1883)
                    .WithClientId(_clientId)
                    .WithCredentials(_mqConfig.mqttUser, _mqConfig.mqttPassword)
                    .WithCleanSession()
                    .Build();
                var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                var subResult = await mqttClient.SubscribeAsync(_mqConfig.mqttTopic);
                subResult.Items.ToList().ForEach(s => _logger.LogInformation($"{_serviceName}:: subscribed to '{s.TopicFilter.Topic}' with '{s.ResultCode}' "));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_serviceName}:: MQtt client error {ex.Message}");
                throw;
            }
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
