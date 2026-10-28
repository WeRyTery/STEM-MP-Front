namespace TechTrend.Api.Models
{
    public class TrendSnapshot
    {
        public int Id { get; set; }
        public int TechnologyId { get; set; }
        public DateTime CollectedAt { get; set; }
        public int ActivityValue { get; set; }
        public double Score { get; set; }
        public string Period { get; set; } = "24h";
        public string Source { get; set; } = "GitHub";

        public Technology? Technology { get; set; }
    }
}
