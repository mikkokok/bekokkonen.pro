namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class LowPriceDateTimeRange
    {
        public DateTime start { get; init; }
        public DateTime end { get; init; }
        public int rank { get; init; }
        public double averagePrice { get; init; }
        public bool isDataValid { get; init; }
    }
}