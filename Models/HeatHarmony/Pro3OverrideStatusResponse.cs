namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class Pro3OverrideStatusResponse
    {
        public bool isOverridden { get; init; }
        public DateTime? until { get; init; }
        public int? outputAmount { get; init; }
        public bool? outputState { get; init; }
    }
}
