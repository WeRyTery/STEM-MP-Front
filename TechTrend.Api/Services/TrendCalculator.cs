namespace TechTrend.Api.Services
{
    public class TrendCalculator
    {
        public double CalculateChange(double previous, double current)
        {
            if (previous <= 0)
                return 0;

            return (current - previous) / previous * 100;
        }

        public double CalculateScore(double changePercent)
        {
            // To ensure 0 to 100 range
            return Math.Clamp(50 + changePercent * 2, 0, 100);
        }

        public double PredictNextScore(double currentScore, double changePercent)
        {
            return Math.Clamp(currentScore + changePercent * 0.2, 0, 100);
        }

        public string GetRecommendation(double score, double changePercent)
        {
            if (score >= 70 && changePercent > 0)
            {
                return "High activity and positive dynamics";

            }
            if (changePercent < 0)
            {
                return "Activity has been dropping - research dynamics";
            }
            return "Average activity - compare with alternatives";
        }
    }
}
