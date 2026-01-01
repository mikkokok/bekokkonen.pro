namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class TRVLatestResponse
    {
        public IEnumerable<ShellyTRV>? devices { get; init; }
        public DateTime serverTime { get; init; }
    }
}