namespace MudBlazorServer.ViewModels
{
    public class TrendViewModel
    {
        public string Trend { get; set; }
        public string Context { get; set; }        // Why it’s trending
        public string ActionableIdea { get; set; } // Specific suggestion
        public string PreDraftedContent { get; set; } // Ready-to-post text
        public int EstimatedReach { get; set; }    // Fake metric for now
    }
}