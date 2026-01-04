namespace bekokkonen.pro.Models.HeatHarmony
{
    public sealed class EmOverrideStatusResponse
    {
        #pragma warning disable IDE1006 // Naming Styles
        public EMOverrideMode OverrideMode { get; set; }
        public bool IsOverrideActive { get; set; }
        public DateTime OverrideUntil { get; set; }
    }
}
