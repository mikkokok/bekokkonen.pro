namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class HeishaMonLatestResponse
    {
        public double inletTemp { get; init; }
        public double outletTemp { get; init; }
        public int targetTemp { get; init; }
        public DateTime serverTime { get; init; }
    }
}