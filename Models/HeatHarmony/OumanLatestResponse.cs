namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class OumanLatestResponse
    {
        public double outsideTemp { get; init; }
        public double flowDemand { get; init; }
        public double insideTempDemand { get; init; }
        public double minFlowTemp { get; init; }
        public bool autoTemp { get; init; }
        public double insideTemp { get; init; }
        public DateTime serverTime { get; init; }
    }
}