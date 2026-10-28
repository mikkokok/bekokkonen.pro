using System.Text.Json.Serialization;

namespace bekokkonen.pro.Models
{
    public sealed class Shelly1PMRelaySettingsResponse
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("appliance_type")]
        public string? ApplianceType { get; set; }

        [JsonPropertyName("ison")]
        public bool IsOn { get; set; }

        [JsonPropertyName("has_timer")]
        public bool HasTimer { get; set; }

        [JsonPropertyName("default_state")]
        public string? DefaultState { get; set; }

        [JsonPropertyName("btn_type")]
        public string? BtnType { get; set; }

        [JsonPropertyName("btn_reverse")]
        public int BtnReverse { get; set; }

        [JsonPropertyName("auto_on")]
        public double AutoOn { get; set; }

        [JsonPropertyName("auto_off")]
        public double AutoOff { get; set; }

        [JsonPropertyName("schedule")]
        public bool Schedule { get; set; }

        [JsonPropertyName("schedule_rules")]
        public required List<string> ScheduleRules { get; set; }

        [JsonPropertyName("max_power")]
        public int MaxPower { get; set; }
    }
}
