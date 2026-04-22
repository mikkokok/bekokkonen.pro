namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class HeishaMonLatestResponse
    {
        public double inletTemp { get; init; }
        public double outletTemp { get; init; }
        public int targetTemp { get; init; }
        public int quietMode { get; init; }
        public double pumpFlow { get; init; }
        public string pumpError { get; init; } = string.Empty;
        public int heatEnergyProduction { get; init; }
        public int heatEnergyConsumption { get; init; }
        public int compressorFrequency { get; init; }
        public DateTime serverTime { get; init; }
    }
}