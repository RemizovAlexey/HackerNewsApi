using HackerNews.Core.Models;

namespace HackerNews.Core.Services
{
    public interface IStoryService
    {
        Task<IEnumerable<Story>> GetBestStories(int count);
    }
}
