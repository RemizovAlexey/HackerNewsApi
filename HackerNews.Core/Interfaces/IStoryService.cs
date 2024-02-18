using HackerNews.Core.Models;

namespace HackerNews.Core.Services
{
    public interface IStoryService
    {
        Task<IEnumerable<Story>> GetStoryDetailsAsync(IEnumerable<int> storyIds);

        Task<IEnumerable<int>> GetBestStoryIdsWithRetryAsync(int maxAttempts);
    }
}
