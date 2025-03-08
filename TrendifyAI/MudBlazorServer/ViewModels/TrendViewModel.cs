namespace MudBlazorServer.ViewModels
{
    public class TrendViewModel
    {
        public string Trend { get; set; }
        public string Context { get; set; }
        public string ActionableIdea { get; set; }
        public string PreDraftedContent { get; set; }
        public int EstimatedReach { get; set; }
        public int Ranking { get; set; } // New field for trend ranking
    }
}