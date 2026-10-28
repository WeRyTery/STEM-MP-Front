using System.Net.Http.Headers;
using System.Text.Json;

namespace TechTrend.Api.Services
{
    public class DataCollector(HttpClient httpClient, IConfiguration configuration)
    {
        public async Task<int> GetActivityAsync(string language, string period, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var duration = period switch
            {
                "24h" => TimeSpan.FromHours(24),
                "7d" => TimeSpan.FromDays(7),
                "30d" => TimeSpan.FromDays(30),
                _ => TimeSpan.FromHours(24)
            };

            var since = now.Subtract(duration).ToString("yyyy-MM-ddTHH:mm:ss");
            var query = Uri.EscapeDataString($"language:{language} pushed:>={since}");

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/search/repositories?q={query}&per_page=1");
            request.Headers.UserAgent.ParseAdd("TechTrend/1.0");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

            var token = configuration["GitHub:Token"];
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            using var response = await httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
            return json.RootElement.GetProperty("total_count").GetInt32();
        }


        public async Task<int> GetStackOverflowActivityAsync(string tag, string period, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var duration = period switch
            {
                "24h" => TimeSpan.FromHours(24),
                "7d" => TimeSpan.FromDays(7),
                "30d" => TimeSpan.FromDays(30),
                _ => TimeSpan.FromHours(24)
            };

            var fromDate = now.Subtract(duration).ToUnixTimeSeconds();
            var toDate = now.ToUnixTimeSeconds();

            var url =
                "https://api.stackexchange.com/2.3/questions" +
                $"?site=stackoverflow&fromdate={fromDate}&todate={toDate}" +
                $"&tagged={Uri.EscapeDataString(tag)}&pagesize=1&filter=total";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.ParseAdd("TechTrend/1.0");
            request.Headers.Accept.ParseAdd("application/json");

            using var response = await httpClient.SendAsync(request, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Stack Exchange API returned {(int)response.StatusCode}: {responseBody}");
            }

            using var json = JsonDocument.Parse(responseBody);
            if (json.RootElement.TryGetProperty("error_message", out var error))
            {
                throw new HttpRequestException($"Stack Exchange API: {error.GetString()}");
            }

            return json.RootElement.GetProperty("total").GetInt32();
        }


        public async Task<int> GetHimalayasJobsAsync(string keyword, CancellationToken cancellationToken = default)
        {
            var apiKey = configuration["Himalayas:ApiKey"];

            using var request = new HttpRequestMessage(HttpMethod.Get, $"https://himalayas.app/jobs/api/search?q={Uri.EscapeDataString(keyword)}&sort=recent");
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            }

            using var response = await httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            using var json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
            return json.RootElement.GetProperty("totalCount").GetInt32();
        }
    }
}