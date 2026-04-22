namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class HeatAutomationOverrideStatusResponse
    {
        public bool isActive { get; init; }
        public double targetTemp { get; init; }
        public DateTime? until { get; init; }
        public DateTime serverTime { get; init; }
    }
}
