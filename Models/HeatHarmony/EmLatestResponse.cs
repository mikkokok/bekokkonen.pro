namespace bekokkonen.pro.Models.HeatHarmony
{
#pragma warning disable IDE1006 // Naming Styles
    public sealed class EmLatestResponse
    {
        public DateTime LastEnabled { get; set; }
        public bool IsOverridden { get; set; }
        public bool isRunning { get; set; }
        public bool IsOn { get; set; }
    }
}
