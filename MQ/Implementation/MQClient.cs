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
        private ConsumptionData? _consumptionData;

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
                _consumptionData ??= new ConsumptionData
                    {
                        Timestamp = DateTime.Now,
                        Data = []
                    };

                switch (applicationMessage.Topic)
                {
                    case "p1meter/actual_consumption":
                        _consumptionData.Data.Add(ConsumptionKeys.ActualConsumption, consumptionValue);
                        break;
                    case "p1meter/actual_returndelivery":
                        _consumptionData.Data.Add(ConsumptionKeys.ActualReturndelivery, consumptionValue);
                        break;
                    case "p1meter/l1_instant_power_usage":
                        _consumptionData.Data.Add(ConsumptionKeys.L1InstantPowerUsage, consumptionValue);
                        break;
                    case "p1meter/l2_instant_power_usage":
                        _consumptionData.Data.Add(ConsumptionKeys.L2InstantPowerUsage, consumptionValue);
                        break;
                    case "p1meter/l3_instant_power_usage":
                        _consumptionData.Data.Add(ConsumptionKeys.L3InstantPowerUsage, consumptionValue);
                        break;
                    case "p1meter/l1_instant_power_current":
                        _consumptionData.Data.Add(ConsumptionKeys.L1InstantPowerCurrent, consumptionValue);
                        break;
                    case "p1meter/l2_instant_power_current":
                        _consumptionData.Data.Add(ConsumptionKeys.L2InstantPowerCurrent, consumptionValue);
                        break;
                    case "p1meter/l3_instant_power_current":
                        _consumptionData.Data.Add(ConsumptionKeys.L3InstantPowerCurrent, consumptionValue);
                        break;
                    case "p1meter/l1_voltage":
                        _consumptionData.Data.Add(ConsumptionKeys.L1Voltage, consumptionValue);
                        break;
                    case "p1meter/l2_voltage":
                        _consumptionData.Data.Add(ConsumptionKeys.L2Voltage, consumptionValue);
                        break;
                    case "p1meter/l3_voltage":
                        _consumptionData.Data.Add(ConsumptionKeys.L3Voltage, consumptionValue);
                        break;
                    case "p1meter/cumulative_power_consumption":
                        _consumptionData.Data.Add(ConsumptionKeys.CumulativePowerConsumption, consumptionValue);
                        break;
                    case "p1meter/cumulative_power_yield":
                        _consumptionData.Data.Add(ConsumptionKeys.CumulativePowerYield, consumptionValue);
                        break;
                    default:
                        _logger.LogInformation($"{_serviceName}:: Received message {payload} in {applicationMessage.Topic}");
                        break;
                }

                if (_consumptionData?.Data.Count == 13)
                {
                    _logger.LogInformation($"{_serviceName}:: Sending {_consumptionData.Timestamp} updated message to broadcastConsumptionData");
                    await _consumptionHub.Clients.All.SendAsync("broadcastConsumptionData", _consumptionData);
                    _consumptionData = null;
                }
            }
        }
    }
}
