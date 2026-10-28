
using TechTrend.Api.Services;
using Xunit;

namespace TechTrend.Api.Tests
{
    public class TrendCalculatorTests
    {
        private readonly TrendCalculator _calculator = new();

        // CalculateChange: activity increased
        [Fact]
        public void CalculateChange_WhenActivityIncreases_ReturnsPositivePercent()
        {
            double result = _calculator.CalculateChange(100, 150);

            Assert.Equal(50, result);
        }

        // CalculateChange: activity decreased
        [Fact]
        public void CalculateChange_WhenActivityDecreases_ReturnsNegativePercent()
        {
            double result = _calculator.CalculateChange(100, 70);

            Assert.Equal(-30, result);
        }

        // CalculateChange: valus are equal
        [Fact]
        public void CalculateChange_WhenValuesAreEqual_ReturnsZero()
        {
            double result = _calculator.CalculateChange(100, 100);

            Assert.Equal(0, result);
        }

        // CalculateChange: last value is zero
        [Fact]
        public void CalculateChange_WhenPreviousIsZero_ReturnsZero()
        {
            double result = _calculator.CalculateChange(0, 50);

            Assert.Equal(0, result);
        }

        // CalculateScore: result clamped between 0 and 100
        [Theory]
        [InlineData(100, 100)]
        [InlineData(-100, 0)]
        [InlineData(0, 50)]
        public void CalculateScore_ReturnsExpectedScore(double changePercent, double expected)
        {
            double result = _calculator.CalculateScore(changePercent);

            Assert.Equal(expected, result);
        }

        // PredictNextScore: prediction within 100 limit
        [Fact]
        public void PredictNextScore_WhenResultExceeds100_Returns100()
        {
            double result = _calculator.PredictNextScore(99, 20);

            Assert.Equal(100, result);
        }

        // GetRecommendation: hight activity and positive grow
        [Fact]
        public void GetRecommendation_WhenScoreIsHighAndChangeIsPositive_ReturnsHighActivity()
        {
            string result = _calculator.GetRecommendation(80, 10);

            Assert.Equal("High activity and positive dynamics", result);
        }

        // GetRecommendation: activity drops
        [Fact]
        public void GetRecommendation_WhenChangeIsNegative_ReturnsWarning()
        {
            string result = _calculator.GetRecommendation(80, -10);

            Assert.Equal(
                "Activity has been dropping - research dynamics",
                result);
        }
    }
}
