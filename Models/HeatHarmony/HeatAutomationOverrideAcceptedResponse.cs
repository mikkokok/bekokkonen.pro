namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class HeatAutomationOverrideAcceptedResponse
    {
        public required string message { get; init; }
        public double temperature { get; init; }
        public int hours { get; init; }
        public int delayHours { get; init; }
        public int? quietMode { get; init; }
        public DateTime requestedAt { get; init; }
    }
}
