using System.Net.Http;
using System.Text.Json;
using Humanizer;
using Microsoft.Data.SqlClient;
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
            await Task.Delay(500); // Simulate API delay
            List<TrendViewModel> trends;
            if (niche.ToLower() == "wnba")
            {
                trends = new List<TrendViewModel>
                {
                    new() { Trend = "#WNBAPlayoffs", Context = "Spiked after last night’s Finals win by the Liberty.", ActionableIdea = "Run a ‘Pick Your MVP’ poll to boost engagement.", PreDraftedContent = "Who’s your #WNBAPlayoffs MVP? Vote now and join the hype!", EstimatedReach = 2200 },
                    new() { Trend = "#LIGHTITUPNYL", Context = "Fans are buzzing over New York’s playoff run.", ActionableIdea = "Post a highlight reel of NYL’s top plays.", PreDraftedContent = "#LIGHTITUPNYL is on fire! Check out these clutch moments—thoughts?", EstimatedReach = 1950 },
                    new() { Trend = "#WNBARising", Context = "New talent is stealing the spotlight this season.", ActionableIdea = "Spotlight a rookie in a quick video.", PreDraftedContent = "#WNBARising stars are killing it—meet [Rookie Name]! Who’s your fave?", EstimatedReach = 1800 },
                    new() { Trend = "#FinalsFever", Context = "Playoff excitement is at an all-time high.", ActionableIdea = "Host a live watch party on X.", PreDraftedContent = "Got #FinalsFever? Join our WNBA watch party tonight—bring your predictions!", EstimatedReach = 1600 },
                    new() { Trend = "#WNBABuzz", Context = "Post-game debates are heating up online.", ActionableIdea = "Start a ‘Best Play’ debate thread.", PreDraftedContent = "#WNBABuzz: What’s the play of the night? Drop your take below!", EstimatedReach = 1400 },
                    new() { Trend = "#LibertyLoyal", Context = "NY Liberty fans are rallying hard.", ActionableIdea = "Launch a fan art contest for team pride.", PreDraftedContent = "Show your #LibertyLoyal spirit—submit your fan art for a chance to win!", EstimatedReach = 1200 },
                    new() { Trend = "#SkyVsFever", Context = "Rivalry game hype is trending big.", ActionableIdea = "Poll fans on the rivalry winner.", PreDraftedContent = "#SkyVsFever showdown—who’s taking it? Vote now!", EstimatedReach = 1000 },
                    new() { Trend = "#WNBAMoment", Context = "Viral plays are sparking conversation.", ActionableIdea = "Share a slow-mo clip of a big moment.", PreDraftedContent = "Relive this #WNBAMoment—[Player]’s clutch shot in slow-mo!", EstimatedReach = 850 },
                    new() { Trend = "#SheGotGame", Context = "Women’s hoops empowerment is trending.", ActionableIdea = "Highlight a player’s story in a thread.", PreDraftedContent = "#SheGotGame: [Player]’s journey to the top—read her story!", EstimatedReach = 700 },
                    new() { Trend = "#WNBATop10", Context = "Weekly highlight lists are gaining traction.", ActionableIdea = "Post your own Top 10 plays list.", PreDraftedContent = "Our #WNBATop10 plays this week—agree? Share yours!", EstimatedReach = 600 }
                };
            }
            else
            {
                trends = new List<TrendViewModel>
                {
                    new() { Trend = $"#{niche}Trend", Context = $"{niche} is hot after a big week.", ActionableIdea = $"Launch a {niche} fan challenge.", PreDraftedContent = $"#{niche}Trend is live! Join our {niche} challenge—details here!", EstimatedReach = 1200 },
                    new() { Trend = $"#{niche}Buzz", Context = $"Fans are hyped about {niche} news.", ActionableIdea = $"Post a {niche} reaction video.", PreDraftedContent = $"#{niche}Buzz—what’s your take on the latest? Share below!", EstimatedReach = 1000 },
                    new() { Trend = $"#{niche}Now", Context = $"Recent {niche} events are trending.", ActionableIdea = $"Run a {niche} live Q&A.", PreDraftedContent = $"#{niche}Now—ask us anything about {niche} in our live Q&A!", EstimatedReach = 900 },
                    new() { Trend = $"#{niche}Vibes", Context = $"{niche} community is energized.", ActionableIdea = $"Create a {niche} meme contest.", PreDraftedContent = $"#{niche}Vibes—drop your best {niche} meme for a shoutout!", EstimatedReach = 800 },
                    new() { Trend = $"#{niche}Talk", Context = $"{niche} debates are sparking up.", ActionableIdea = $"Start a {niche} opinion poll.", PreDraftedContent = $"#{niche}Talk—what’s your stance on [topic]? Vote now!", EstimatedReach = 700 },
                    new() { Trend = $"#{niche}Live", Context = $"Live {niche} updates are trending.", ActionableIdea = $"Share a {niche} live update.", PreDraftedContent = $"#{niche}Live—[update] just dropped! Thoughts?", EstimatedReach = 600 },
                    new() { Trend = $"#{niche}Fans", Context = $"{niche} fanbase is growing loud.", ActionableIdea = $"Host a {niche} fan meetup.", PreDraftedContent = $"#{niche}Fans—join our virtual meetup this weekend!", EstimatedReach = 500 },
                    new() { Trend = $"#{niche}Peak", Context = $"{niche} hit a peak moment.", ActionableIdea = $"Post a {niche} throwback.", PreDraftedContent = $"#{niche}Peak—remember this [moment]? Relive it!", EstimatedReach = 400 },
                    new() { Trend = $"#{niche}Next", Context = $"{niche} future is trending.", ActionableIdea = $"Predict the next {niche} big thing.", PreDraftedContent = $"#{niche}Next—what’s coming up in {niche}? Your guess?", EstimatedReach = 300 },
                    new() { Trend = $"#{niche}Win", Context = $"{niche} victories are buzzing.", ActionableIdea = $"Celebrate a {niche} win.", PreDraftedContent = $"#{niche}Win—huge congrats to [winner]! Celebrate with us!", EstimatedReach = 200 }
                };
            }

            // Sort by EstimatedReach descending and assign rankings
            trends = trends.OrderByDescending(t => t.EstimatedReach).ToList();
            for (int i = 0; i < trends.Count; i++)
            {
                trends[i].Ranking = i + 1;
            }

            return trends;
        }
        //public async Task<List<TrendViewModel>> GetTrends(string niche)
        //{
        //    try
        //    {
        //        if (_cache.TryGetValue(niche, out var cachedTrends))
        //        {
        //            return cachedTrends;
        //        }

        //        var response = await _httpClient.GetAsync($"tweets/search/recent?query={niche}&max_results=10");
        //        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        //        {
        //            // Log rate limit headers for testing
        //            var headers = response.Headers;
        //            var limit = headers.GetValues("X-Rate-Limit-Limit").FirstOrDefault() ?? "Unknown";
        //            var remaining = headers.GetValues("X-Rate-Limit-Remaining").FirstOrDefault() ?? "Unknown";
        //            var reset = headers.GetValues("X-Rate-Limit-Reset").FirstOrDefault() ?? "Unknown";

        //            Console.WriteLine("Rate Limit Hit (429):");
        //            Console.WriteLine($"X-Rate-Limit-Limit: {limit} (Total allowed requests)");
        //            Console.WriteLine($"X-Rate-Limit-Remaining: {remaining} (Requests left)");
        //            Console.WriteLine($"X-Rate-Limit-Reset: {reset} (Epoch time - convert at epochconverter.com)");



        //            return new List<TrendViewModel>
        //                {
        //                    new() { Trend = "Rate Limit Hit", SuggestedContent = "Too many requests—try again later!" }
        //                };
        //        }
        //        response.EnsureSuccessStatusCode();

        //        var json = await response.Content.ReadAsStringAsync();
        //        var data = JsonSerializer.Deserialize<XApiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        //        var trends = data?.Data?
        //            .SelectMany(t => t.Text.Split(' ')
        //                .Where(w => w.StartsWith("#")))
        //            .GroupBy(t => t)
        //            .OrderByDescending(g => g.Count())
        //            .Take(2)
        //            .Select(g => new TrendViewModel
        //            {
        //                Trend = g.Key,
        //                SuggestedContent = $"Post about {g.Key} now!"
        //            })
        //            .ToList() ?? new List<TrendViewModel>();

        //        return trends.Any() ? trends : new List<TrendViewModel>
        //            {
        //                new() { Trend = "No Trends", SuggestedContent = $"No {niche} trends found" }
        //            };
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        throw new Exception($"API request failed: {ex.Message}");
        //    }
        //}

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



//Clarify X API Endpoint Behavior
//You asked about await _httpClient.GetAsync($"tweets/search/recent?query={niche}&max_results=10");—does it return the first 10 results or the 10 most popular results? Let’s break it down:
//X API Docs: The / tweets / search / recent endpoint(v2) returns tweets from the last 7 days matching the query.  

//Default Behavior: Without a sort_order parameter, it returns the most recent tweets (not necessarily most popular).  
//max_results=10 means it grabs the 10 most recent tweets for {niche} (e.g., “WNBA”).

//Popularity: To get the “most popular” (e.g., by retweets/likes), you’d need:  
//sort_order = popularity(not available in Free tier—requires Academic or Enterprise access).

//Or post-process the results yourself (e.g., sort by public_metrics like retweet_count, which we can’t do with Free tier’s 1-request limit).

//Your Code: With max_results = 10, it fetched 10 recent tweets, then your logic picked the top 2 hashtags by frequency—not popularity (since we didn’t access metrics).

//Answer: It returns the first 10 most recent results, not the 10 most popular. Our dummy data mimics this by giving static trends, now ranked by EstimatedReach.

