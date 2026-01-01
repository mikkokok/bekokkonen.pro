namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class OumanStatusResponse
    {
        public IEnumerable<HarmonyChange>? changes { get; init; }
        public DateTime serverTime { get; init; }
    }
}