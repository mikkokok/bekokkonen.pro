using bekokkonen.pro.Global;
using bekokkonen.pro.Global.Config;
using bekokkonen.pro.Global.Enum;
using bekokkonen.pro.Models;

namespace bekokkonen.pro.Providers
{
    public sealed class Shelly1PMProvider
    {
        private readonly ILogger<Shelly1PMProvider> _logger;
        private readonly IRequestProvider _requestProvider;
        private readonly string _serviceName = nameof(Shelly1PMProvider);
        public Task Shelly1PMProviderStatusTask { get; private set; }

        public Shelly1PMProvider(ILogger<Shelly1PMProvider> logger, IRequestProvider requestProvider)
        {
            _logger = logger;
            _requestProvider = requestProvider;
            Shelly1PMProviderStatusTask = Task.Run(GetShelly1PMRelayStatus);
        }

        private async Task GetShelly1PMRelayStatus()
        {
            while (true)
            {
                try
                {
                    foreach (var device in GlobalConfig.Shelly1PMConfig!.Devices.Values)
                    {
                        var url = $"http://{device.IPAddress}/relay/0";
                        var result = await _requestProvider.GetAsync<Shelly1PMRelayResponse>(HttpClientConst.Shelly1PM, url);
                        if (result != null)
                        {
                            device.RelayStatus = result.IsOn;
                        }
                        else
                        {
                            _logger.LogWarning("{service}:: GetShelly1PMRelayStatus returned null", _serviceName);
                        }

                    }
                    await Task.Delay(TimeSpan.FromMinutes(5));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "{service}:: GetShelly1PMRelayStatus exception", _serviceName);
                    await Task.Delay(TimeSpan.FromMinutes(1));
                }
            }
        }

        public List<GlobalConfig.Shelly1PMDevice> GetShelly1PMDevices()
        {
            try
            {
                var shellyConfig = GlobalConfig.Shelly1PMConfig;
                if (shellyConfig is null)
                {
                    _logger.LogError("Shelly1PM configuration is not available.");
                    return [];
                }
                return [.. shellyConfig.Devices.Values];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving Shelly1PM devices.");
                return [];
            }
        }

        public async Task SetShelly1PMRelay(Shelly1PMDeviceName deviceName, bool turnOn = true)
        {
            try
            {
                var shellyConfig = GlobalConfig.Shelly1PMConfig;
                if (shellyConfig is null || !shellyConfig.Devices.TryGetValue(deviceName, out var device))
                {
                    _logger.LogError("Shelly1PM device '{DeviceName}' not found in configuration.", deviceName);
                    return;
                }

                var turnValue = turnOn ? "on" : "off";
                var url = $"http://{device.IPAddress}/relay/0?turn={turnValue}";
                var response = await _requestProvider.GetAsync<Shelly1PMRelayResponse>(HttpClientConst.Shelly1PM, url) ?? throw new Exception($"Failed to get response from Shelly1PM {deviceName} device. ");
                device.RelayStatus = response.IsOn;
                if (response.IsOn != turnOn)
                {
                    _logger.LogWarning("Failed to set Shelly1PM relay for '{DeviceName}' to '{TurnValue}'. Current status: {CurrentStatus}.", deviceName, turnValue, response.IsOn ? "on" : "off");
                }
                else
                {
                    _logger.LogInformation("Successfully set Shelly1PM relay for '{DeviceName}' to '{TurnValue}'.", deviceName, turnValue);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while setting Shelly1PM relay for '{DeviceName}'.", deviceName);
            }
        }

        public async Task<Shelly1PMRelaySettingsResponse> GetShelly1PMRelaySettings(Shelly1PMDeviceName deviceName)
        {
            try
            {
                var shellyConfig = GlobalConfig.Shelly1PMConfig;
                if (shellyConfig is null || !shellyConfig.Devices.TryGetValue(deviceName, out var device))
                {
                    _logger.LogError("Shelly1PM device '{DeviceName}' not found in configuration.", deviceName);
                    return null!;
                }
                var url = $"http://{device.IPAddress}/settings/relay/0";
                var response = await _requestProvider.GetAsync<Shelly1PMRelaySettingsResponse>(HttpClientConst.Shelly1PM, url) ?? throw new Exception($"Failed to get settings response from Shelly1PM {deviceName} device. ");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting settings for Shelly1PM device '{DeviceName}'.", deviceName);
                throw;
            }
        }

        public async Task<Shelly1PMRelaySettingsResponse> SetShelly1PMRelaySchedule(Shelly1PMDeviceName deviceName, string schedule, bool add = true)
        {
            try
            {
                var shellyConfig = GlobalConfig.Shelly1PMConfig;
                if (shellyConfig is null || !shellyConfig.Devices.TryGetValue(deviceName, out var device))
                {
                    _logger.LogError("Shelly1PM device '{DeviceName}' not found in configuration.", deviceName);
                    throw new Exception($"Shelly1PM device '{deviceName}' not found in configuration.");
                }
                var existingRules = await GetShelly1PMRelaySettings(deviceName);
                var rules = (existingRules.ScheduleRules ?? [])
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Distinct(StringComparer.Ordinal)
                            .ToList();

                if (!rules.Contains(schedule, StringComparer.Ordinal) && add)
                {
                    rules.Add(schedule);
                }
                else if (rules.Contains(schedule, StringComparer.Ordinal) && !add)
                {
                    rules.Remove(schedule);
                }

                var encodedRules = Uri.EscapeDataString(string.Join(",", rules));
                var url = $"http://{device.IPAddress}/settings/relay/0?schedule_rules={encodedRules}";
                var response = await _requestProvider.GetAsync<Shelly1PMRelaySettingsResponse>(HttpClientConst.Shelly1PM, url) ?? throw new Exception($"Failed to set schedule {schedule} for Shelly1PM {deviceName} device. ");
                _logger.LogInformation("Successfully set Shelly1PM relay settings for '{DeviceName}'.", deviceName);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while setting settings for Shelly1PM device '{DeviceName}'.", deviceName);
                throw;
            }
        }
    }
}