using MudBlazorServer.ViewModels;

namespace MudBlazorServer.Services.Interfaces
{
    public interface ITrendService
    {
        Task<List<TrendViewModel>> GetTrends(string niche);
    }
}
