namespace bekokkonen.pro.Models
{
    public class ConsumptionData
    {
        public DateTime Timestamp { get; set; }

        public required Dictionary<ConsumptionKeys, double> Data { get; set; }
    }
}
