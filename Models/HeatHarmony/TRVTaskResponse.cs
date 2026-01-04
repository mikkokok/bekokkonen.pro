namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class TRVTaskResponse
    {
        public string? status { get; init; }
        public IEnumerable<string>? errors { get; init; }
        public DateTime serverTime { get; init; }
    }
}