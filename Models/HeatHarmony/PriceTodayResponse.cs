namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class PriceTodayResponse
    {
        public IEnumerable<ElectricityPrice>? prices { get; init; }
    }
}