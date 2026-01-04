namespace bekokkonen.pro.Models.HeatHarmony
{
    public sealed class TemperatureOverride
    {
        public required double Temperature { get; set; }
        public required int Hours { get; set; }
        public required bool OverRidePrevious { get; set; }
        public int Delay { get; set; } = 0;
    }
}
