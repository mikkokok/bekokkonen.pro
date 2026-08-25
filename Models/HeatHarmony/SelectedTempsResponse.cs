namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class SelectedTempsResponse
    {
        public int minTemp { get; init; }
        public int midTemp { get; init; }
        public int maxTemp { get; init; }
        public int maxHeatingPeriodTemp { get; init; }
    }
}
