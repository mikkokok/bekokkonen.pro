namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class HeatAutomationTasksResponse
    {
        public required HeatAutomationTaskStatus oumanAndHeishamonSync { get; init; }
        public required HeatAutomationTaskStatus setUseWaterBasedOnPrice { get; init; }
        public required HeatAutomationTaskStatus setInsideTempBasedOnPrice { get; init; }
        public DateTime serverTime { get; init; }
    }

    public sealed class HeatAutomationTaskStatus
    {
        public string status { get; init; } = string.Empty;
        public string[] errors { get; init; } = [];
    }
}