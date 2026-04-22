namespace bekokkonen.pro.Models.HeatHarmony
{
    public sealed class EmOverrideStatusResponse
    {
        #pragma warning disable IDE1006 // Naming Styles
        public EMOverrideMode overrideMode { get; set; }
        public bool isOverrideActive { get; set; }
        public DateTime? overrideUntil { get; set; }
    }
}
