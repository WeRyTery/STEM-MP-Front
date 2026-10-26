using System.Net.Http.Headers;
using System.Text.Json;

namespace TechTrend.Api.Services
{
    public class DataCollector(HttpClient httpClient)
    {
        public async Task<int> GetActivityAsync(string language, CancellationToken cancellationToken = default)
        {
            var since = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd");

            var query = Uri.EscapeDataString($"language:{language} pushed:>={since}");

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/search/repositories?q={query}&per_page=1");
            request.Headers.UserAgent.ParseAdd("TechTrend/1.0");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

            using var response = await httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var json = await JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync(cancellationToken),
                cancellationToken: cancellationToken);

            return json.RootElement.GetProperty("total_count").GetInt32();
        }
    }
}
