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
    public sealed class MQClient(ILogger<MQClient> logger, IHubContext<ConsumptionHub, IConsumptionHubClient> electricityHub)
    {
        private string _serviceName = nameof(MQClient);
        private readonly string _clientId = "bekokkonenproot";
        private readonly ILogger<MQClient> _logger = logger;
        private readonly GlobalConfig.RabbitMQ _mqConfig = GlobalConfig.RabbitMQConfig!;
        private readonly IHubContext<ConsumptionHub, IConsumptionHubClient> _consumptionHub = electricityHub;
        private ConsumptionData? _consumptionData;
        private WemosData? _wemosData;
        private readonly List<ConsumptionData> _consumptionDataHistoryList = [];
        private readonly List<WemosData> _wemosDataHistoryList = [];
        public MQStatusEnum Status { get; private set; } = MQStatusEnum.Disconnected;

        public Task? Initialization;

        public List<ConsumptionData> GetConsumptionDataHistory()
        {
            return _consumptionDataHistoryList;
        }
        public List<WemosData> GetWemosDataHistory()
        {
            return _wemosDataHistoryList;
        }

        public async Task InitializeMqttClient()
        {
            _logger.LogInformation("{ServiceName}:: Initialize MQtt client", _serviceName);
            Status = MQStatusEnum.Connecting;
            try
            {
                var mqttClient = new MqttClientFactory().CreateMqttClient();
                mqttClient.ApplicationMessageReceivedAsync += m => HandleMessage(m.ApplicationMessage);
                mqttClient.DisconnectedAsync += e =>
                {
                    _logger.LogWarning("{ServiceName}:: MQtt client disconnected: {Reason}", _serviceName, e.Reason);
                    Status = MQStatusEnum.Disconnected;
                    return Task.CompletedTask;
                };

                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(_mqConfig.mqttServer, 1883)
                    .WithClientId(_clientId)
                    .WithCredentials(_mqConfig.mqttUser, _mqConfig.mqttPassword)
                    .WithCleanSession()
                    .Build();

                var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                var subResult = await mqttClient.SubscribeAsync(_mqConfig.mqttTopic);
                subResult.Items
                    .ToList()
                    .ForEach(s => _logger.LogInformation(
                        "{ServiceName}:: Subscribed to '{Topic}' with '{ResultCode}'",
                        _serviceName,
                        s.TopicFilter.Topic,
                        s.ResultCode));
            }
            catch (Exception ex)
            {
                Status = MQStatusEnum.Error;
                _logger.LogError(ex, "{ServiceName}:: MQtt client error {ErrorMessage}", _serviceName, ex.Message);
                throw;
            }
            Status = MQStatusEnum.Connected;
            _logger.LogInformation("{ServiceName}:: MQtt client connected successfully", _serviceName);
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
                _wemosData ??= new WemosData
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
                    case "p1meter/free_heap":
                        _wemosData.Data.Add(WemosDataKeys.FreeHeap, consumptionValue);
                        break;
                    case "p1meter/heap_fragmentation":
                        _wemosData.Data.Add(WemosDataKeys.HeapFragmentation, consumptionValue);
                        break;
                    case "p1meter/max_free_block":
                        _wemosData.Data.Add(WemosDataKeys.MaxFreeBlock, consumptionValue);
                        break;
                    case "p1meter/uptime_sec":
                        _wemosData.Data.Add(WemosDataKeys.Uptime, consumptionValue);
                        break;
                    case "p1meter/wifi_rssi":
                        _wemosData.Data.Add(WemosDataKeys.WifiSignalStrength, consumptionValue);
                        break;
                    default:
                        _logger.LogInformation(
                            "{ServiceName}:: Received message {Payload} in {Topic}",
                            _serviceName,
                            payload,
                            applicationMessage.Topic);
                        break;
                }

                if (_consumptionData?.Data.Count == 13)
                {
                    _logger.LogInformation(
                        "{ServiceName}:: Sending {Timestamp} updated message to broadcastConsumptionData",
                        _serviceName,
                        _consumptionData.Timestamp);

                    await _consumptionHub.Clients.All.BroadcastConsumptionData(_consumptionData);
                    AddConsumptionHistory(_consumptionData);
                    _consumptionData = null;
                }
                if (_wemosData?.Data.Count == 5)
                {
                    AddWemosHistory(_wemosData);
                    _wemosData = null;
                }
            }
        }

        private void AddConsumptionHistory(ConsumptionData consumptionData)
        {
            _consumptionDataHistoryList.Add(consumptionData);
            _consumptionDataHistoryList.RemoveAll(cd => cd.Timestamp < DateTime.Now.AddDays(-7));
        }

        private void AddWemosHistory(WemosData wemosData)
        {
            _wemosDataHistoryList.Add(wemosData);
            _wemosDataHistoryList.RemoveAll(cd => cd.Timestamp < DateTime.Now.AddDays(-7));
        }
    }
}
