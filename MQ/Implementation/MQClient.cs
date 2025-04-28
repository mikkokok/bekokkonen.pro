using bekokkonen.pro.Global.Config;
using bekokkonen.pro.Global.Interfaces;
using bekokkonen.pro.Models;
using bekokkonen.pro.Routes.Hubs;
using Microsoft.AspNetCore.SignalR;
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
        private IHubContext<ConsumptionHub> _consumptionHub;

        public MQClient(ILogger<MQClient> logger, IHubContext<ConsumptionHub> electricityHub)
        {
            _serviceName = nameof(MQClient);
            _logger = logger;
            _mqConfig = GlobalConfig.RabbitMQConfig!;
            _consumptionHub = electricityHub;
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
                subResult.Items.ToList().ForEach(s => _logger.LogInformation($"{_serviceName}:: Subscribed to '{s.TopicFilter.Topic}' with '{s.ResultCode}' "));
            }
            catch (Exception ex)
            {
                _logger.LogError($"{_serviceName}:: MQtt client error {ex.Message}");
                throw;
            }
            _logger.LogInformation($"{_serviceName}:: MQtt client connected successfully");
        }

        private async Task HandleMessage(MqttApplicationMessage applicationMessage)
        {
            var payload = Encoding.UTF8.GetString(applicationMessage.Payload);
            if (double.TryParse(payload, out double consumptionValue))
            {
                var dataToSend = new ConsumptionData
                {
                    Timestamp = DateTime.Now,
                    Unit = "Wh",
                    Value = consumptionValue
                };

                switch (applicationMessage.Topic)
                {
                    case "p1meter/actual_consumption":
                        _logger.LogInformation($"{_serviceName}:: Received message {payload} in p1meter/actual_consumption");
                        await _consumptionHub.Clients.All.SendAsync("broadcastActualConsumption", dataToSend);
                        break;
                    case "p1meter/actual_returndelivery":
                        _logger.LogInformation($"{_serviceName}:: Received message {payload} in p1meter/actual_returndelivery");
                        await _consumptionHub.Clients.All.SendAsync("broadcastReturnDelivery", dataToSend);
                        break;
                    default:
                        _logger.LogInformation($"{_serviceName}:: Received message {payload} in {applicationMessage.Topic}");
                        break;
                }
            }
        }
    }
}
