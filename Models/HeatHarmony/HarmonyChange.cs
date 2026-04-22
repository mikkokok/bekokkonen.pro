using System.Text.Json.Serialization;

namespace bekokkonen.pro.Models.HeatHarmony
{
    public sealed class HarmonyChange
    {
        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("provider")]
        public HeatHarmonyProvider Provider { get; set; }

        [JsonPropertyName("changeType")]
        public HarmonyChangeType ChangeType { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }

    public enum HarmonyChangeType
    {
        InsideTemp,
        DisableWaterHeating,
        EnableWaterHeating,
        SetMinFlowTemp,
        SetInsideTemp,
        SetMaximumFlow,
        SetAutoDriveOn,
        SetDefault,
        SetConservativeHeating,
        OverrideEnable,
        SetTargetTemp,
        OilBurnerEnable,
        OilBurnerDisable
    }
}
