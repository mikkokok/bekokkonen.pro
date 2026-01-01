namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class AllLowPeriodsResponse
    {
        public IEnumerable<LowPriceDateTimeRange>? periods { get; init; }
    }
}