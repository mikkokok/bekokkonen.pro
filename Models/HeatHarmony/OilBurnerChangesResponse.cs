namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class OilBurnerChangesResponse
    {
        public required IEnumerable<HarmonyChange> changes { get; init; }
    }
}
