using MudBlazorServer.Models;
using MudBlazorServer.Services.Interfaces;
using MudBlazorServer.ViewModels;

namespace MudBlazorServer.Services
{
    public class TrendService : ITrendService
    {
        public async Task<List<TrendViewModel>> GetTrends(string niche)
        {
            // Simulate API call (X API later)
            await Task.Delay(1000);
            var results = new List<TrendResult>
            {
                new() { Trend = $"#{niche}Trend", SuggestedContent = $"Top 5 {niche} Ideas" },
                new() { Trend = $"#{niche}Buzz", SuggestedContent = $"Why {niche} is Hot Now" }
            };

            // Map Models to ViewModels
            return results.Select(r => new TrendViewModel
            {
                Trend = r.Trend,
                SuggestedContent = r.SuggestedContent
            }).ToList();
        }
    }
}