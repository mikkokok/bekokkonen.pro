namespace bekokkonen.pro.Models
{
    public class ConsumptionData
    {
        public DateTimeOffset Timestamp { get; set; }
        public required double Value { get; set; }
        public required string Unit { get; set; }
    }
}
