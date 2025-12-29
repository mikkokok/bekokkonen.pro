namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class UptimeResponse
    {
        public DateTime startupTime { get; set; }
        public DateTime serverTime { get; set; }
        public required UptimeInfo uptime { get; set; }
    }

    public sealed class UptimeInfo
    {
        public long ticks { get; set; }
        public double totalSeconds { get; set; }
        public required string duration { get; set; }
    }
}
