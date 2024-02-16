using HackerNews.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HackerNewsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly IStoryService _storyService;

        public StoriesController(IStoryService storyService)
        {
            _storyService = storyService;
        }

        [HttpGet("{count}")]
        public async Task<ActionResult<IEnumerable<HackerNews.Core.Models.Story>>> GetBestStories(int count)
        {
            var stories = await _storyService.GetBestStories(count);
            return Ok(stories.OrderByDescending(s => s.Score));
        }
    }
}
