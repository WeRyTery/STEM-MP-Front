namespace TechTrend.Api.Models
{
    public class Technology
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public string Language { get; set; } = "";

        public List<TrendSnapshot> Snapshots { get; set; } = [];
    }
}
