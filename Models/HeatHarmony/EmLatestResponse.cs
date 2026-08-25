namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class EmLatestResponse
    {
        public DateTime lastEnabled { get; set; }
        public DateTime lastDisabled { get; set; }
        public bool isOverridden { get; set; }
        public bool isRunning { get; set; }
        public bool isOn { get; set; }
    }
}
