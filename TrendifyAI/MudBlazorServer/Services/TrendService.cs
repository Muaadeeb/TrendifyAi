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
            try
            {
                var response = await _httpClient.GetAsync($"tweets/search/recent?query={niche}&max_results=10");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<XApiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Extract hashtags as trends
                var trends = data?.Data?
                    .SelectMany(t => t.Text.Split(' ')
                        .Where(w => w.StartsWith("#")))
                    .GroupBy(t => t)
                    .OrderByDescending(g => g.Count())
                    .Take(2)
                    .Select(g => new TrendViewModel
                    {
                        Trend = g.Key,
                        SuggestedContent = $"Post about {g.Key} now!"
                    })
                    .ToList() ?? new List<TrendViewModel>();

                return trends.Any() ? trends : new List<TrendViewModel>
                {
                    new() { Trend = "No Trends", SuggestedContent = $"No {niche} trends found" }
                };
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API request failed: {ex.Message}");
            }
        }
    }

    // Classes for X API response deserialization
    public class XApiResponse
    {
        public List<TweetData>? Data { get; set; }
    }

    public class TweetData
    {
        public string? Text { get; set; }
    }
}