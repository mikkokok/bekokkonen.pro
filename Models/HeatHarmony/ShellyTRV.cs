namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class ShellyTRV
    {
        public string? name { get; init; }
        public string? ip { get; init; }
        public DateTime updatedAt { get; init; }
        public TRVStatusEnum status { get; init; }
        public string? message { get; init; }
        public int batteryLevel { get; init; }
        public double latestLevel { get; init; }
        public bool autoTemperature { get; init; }
    }
}