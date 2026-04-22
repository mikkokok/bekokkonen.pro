namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class HeatAutomationOverrideCancelledResponse
    {
        public required string message { get; init; }
        public DateTime cancelledAt { get; init; }
    }
}
