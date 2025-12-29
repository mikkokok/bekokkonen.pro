namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class PingResponse
    {
        public required string status { get; set; }
        public DateTime serverTime { get; set; }
    }
}
