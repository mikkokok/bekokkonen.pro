namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class ElectricityPrice
    {
        public string? date { get; init; }
        public string? price { get; init; }
        public int hour { get; init; }
    }
}