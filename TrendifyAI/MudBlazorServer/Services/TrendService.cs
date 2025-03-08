using System.Net.Http;
using System.Text.Json;
using MudBlazorServer.Models;
using MudBlazorServer.Services.Interfaces;
using MudBlazorServer.ViewModels;

namespace MudBlazorServer.Services
{
    public class TrendService : ITrendService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        private readonly Dictionary<string, List<TrendViewModel>> _cache = new();

        public TrendService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _httpClient.BaseAddress = new Uri("https://api.twitter.com/2/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new("Bearer", _config["XApi:BearerToken"] ?? throw new InvalidOperationException("X API Bearer Token not found in configuration"));
        }

        public async Task<List<TrendViewModel>> GetTrends(string niche)
        {
            await Task.Delay(1000);
            if (niche.ToLower() == "wnba") // Niche-specific mock
            {
                return new List<TrendViewModel>
                {
                    new()
                    {
                        Trend = "#WNBAPlayoffs",
                        Context = "Spiked after last night’s Finals win by the Liberty.",
                        ActionableIdea = "Run a ‘Pick Your MVP’ poll to boost engagement.",
                        PreDraftedContent = "Who’s your #WNBAPlayoffs MVP? Vote now and join the hype!",
                        EstimatedReach = 2200
                    },
                    new()
                    {
                        Trend = "#LIGHTITUPNYL",
                        Context = "Fans are buzzing over New York’s playoff run.",
                        ActionableIdea = "Post a highlight reel of NYL’s top plays.",
                        PreDraftedContent = "#LIGHTITUPNYL is on fire! Check out these clutch moments—thoughts?",
                        EstimatedReach = 950
                    }
                };
            }
            // Generic fallback
            return new List<TrendViewModel>
            {
                new()
                {
                    Trend = $"#{niche}Trend",
                    Context = $"{niche} is hot after a big week.",
                    ActionableIdea = $"Launch a {niche} fan challenge to ride the wave.",
                    PreDraftedContent = $"#{niche}Trend is live! Join our {niche} challenge—details here!",
                    EstimatedReach = 1200
                }
            };
        }

        //    public async Task<List<TrendViewModel>> GetTrends(string niche)
        //    {
        //        try
        //        {
        //            if (_cache.TryGetValue(niche, out var cachedTrends))
        //            {
        //                return cachedTrends;
        //            }

        //            var response = await _httpClient.GetAsync($"tweets/search/recent?query={niche}&max_results=10");
        //            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        //            {
        //                // Log rate limit headers for testing
        //                var headers = response.Headers;
        //                var limit = headers.GetValues("X-Rate-Limit-Limit").FirstOrDefault() ?? "Unknown";
        //                var remaining = headers.GetValues("X-Rate-Limit-Remaining").FirstOrDefault() ?? "Unknown";
        //                var reset = headers.GetValues("X-Rate-Limit-Reset").FirstOrDefault() ?? "Unknown";

        //                Console.WriteLine("Rate Limit Hit (429):");
        //                Console.WriteLine($"X-Rate-Limit-Limit: {limit} (Total allowed requests)");
        //                Console.WriteLine($"X-Rate-Limit-Remaining: {remaining} (Requests left)");
        //                Console.WriteLine($"X-Rate-Limit-Reset: {reset} (Epoch time - convert at epochconverter.com)");



        //                return new List<TrendViewModel>
        //                {
        //                    new() { Trend = "Rate Limit Hit", SuggestedContent = "Too many requests—try again later!" }
        //                };
        //            }
        //            response.EnsureSuccessStatusCode();

        //            var json = await response.Content.ReadAsStringAsync();
        //            var data = JsonSerializer.Deserialize<XApiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //            var trends = data?.Data?
        //                .SelectMany(t => t.Text.Split(' ')
        //                    .Where(w => w.StartsWith("#")))
        //                .GroupBy(t => t)
        //                .OrderByDescending(g => g.Count())
        //                .Take(2)
        //                .Select(g => new TrendViewModel
        //                {
        //                    Trend = g.Key,
        //                    SuggestedContent = $"Post about {g.Key} now!"
        //                })
        //                .ToList() ?? new List<TrendViewModel>();

        //            return trends.Any() ? trends : new List<TrendViewModel>
        //            {
        //                new() { Trend = "No Trends", SuggestedContent = $"No {niche} trends found" }
        //            };
        //        }
        //        catch (HttpRequestException ex)
        //        {
        //            throw new Exception($"API request failed: {ex.Message}");
        //        }
        //    }
        //
    }

    public class XApiResponse
    {
        public List<TweetData>? Data { get; set; }
    }

    public class TweetData
    {
        public string? Text { get; set; }
    }
}