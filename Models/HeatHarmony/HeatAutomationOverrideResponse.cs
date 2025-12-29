namespace bekokkonen.pro.Models.HeatHarmony
{
    public class HeatAutomationOverrideResponse
    {
#pragma warning disable IDE1006 // Naming Styles
        public string message { get; init; } = string.Empty;
        public double temperature { get; init; }
        public int hours { get; init; }
        public int delayHours { get; init; }
        public DateTime requestedAt { get; init; }
    }
}
