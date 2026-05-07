namespace bekokkonen.pro.Models
{
    public class WemosData
    {
        public DateTime Timestamp { get; set; }
        public required Dictionary<WemosDataKeys, double> Data { get; set; }
    }
}
