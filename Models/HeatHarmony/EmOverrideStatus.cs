namespace bekokkonen.pro.Models.HeatHarmony
{
    public sealed class EmOverrideStatus
    {
        #pragma warning disable IDE1006 // Naming Styles
        public EMOverrideMode OverrideMode { get; set; }
        public bool IsOverrideActive { get; set; }
        public DateTime OverrideUntil { get; set; }
    }
}
