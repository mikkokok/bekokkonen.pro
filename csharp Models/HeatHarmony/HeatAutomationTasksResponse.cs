namespace bekokkonen.pro.Models.HeatHarmony
{
    #pragma warning disable IDE1006 // Naming Styles
    public sealed class HeatAutomationTasksResponse
    {
        public HeatAutomationTaskStatus oumanAndHeishamonSync { get; init; } = default!;
        public HeatAutomationTaskStatus setUseWaterBasedOnPrice { get; init; } = default!;
        public HeatAutomationTaskStatus setInsideTempBasedOnPrice { get; init; } = default!;
        public DateTime serverTime { get; init; }
    }

    public sealed class HeatAutomationTaskStatus
    {
        public string status { get; init; } = string.Empty;
        public string[] errors { get; init; } = Array.Empty<string>();
    }
}