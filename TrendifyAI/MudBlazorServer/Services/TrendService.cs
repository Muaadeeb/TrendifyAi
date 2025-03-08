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
            await Task.Delay(1000); // Simulate async
            return new List<TrendViewModel>
            {
                new()
                {
                    Trend = $"#{niche}Trend",
                    Context = $"{niche} is gaining traction due to recent events.",
                    ActionableIdea = $"Create a {niche}-themed giveaway to ride the wave.",
                    PreDraftedContent = $"Excited for #{niche}Trend! Win big in our {niche} giveaway—enter now!",
                    EstimatedReach = 1500
                },
                new()
                {
                    Trend = $"#{niche}Buzz",
                    Context = $"Fans are hyped about {niche} milestones this week.",
                    ActionableIdea = $"Post a {niche} highlight reel to engage followers.",
                    PreDraftedContent = $"#{niche}Buzz is unreal! Check out these top moments—what’s your fave?",
                    EstimatedReach = 800
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