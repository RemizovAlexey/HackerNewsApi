using AutoMapper;
using HackerNews.Core.Models;
using HackerNews.Data.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace HackerNews.Core.Services
{
    public class StoryService : IStoryService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public StoryService(IHttpClientFactory httpClientFactory, IMapper mapper, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Story>> GetStoryDetailsAsync(IEnumerable<int> storyIds)
        {
            var tasks = storyIds.Select(id => GetStoryDetailWithRetryAsync(id));
            var stories = await Task.WhenAll(tasks);
            return stories.Where(s => s != null);
        }

        public async Task<IEnumerable<int>> GetBestStoryIdsWithRetryAsync(int maxAttempts = 3)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var response = await _httpClient.GetAsync(_configuration.GetValue<string>("BestStory:Beststories"));
                    response.EnsureSuccessStatusCode();

                    var contentStream = await response.Content.ReadAsStreamAsync();
                    var bestStoryIds = await JsonSerializer.DeserializeAsync<IEnumerable<int>>(contentStream, _options);

                    return bestStoryIds;
                }
                catch (HttpRequestException)
                {
                    if (attempt == maxAttempts)
                    {
                        throw;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            return null;
        }

        private async Task<Story> GetStoryDetailWithRetryAsync(int storyId, int maxAttempts = 3)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"{_configuration.GetValue<string>("BestStory:Item")}/{storyId}.json");
                    response.EnsureSuccessStatusCode();

                    using var contentStream = await response.Content.ReadAsStreamAsync();
                    var storyDto = await JsonSerializer.DeserializeAsync<StoryDto>(contentStream, _options);

                    return _mapper.Map<Story>(storyDto);
                }
                catch (HttpRequestException)
                {
                    if (attempt == maxAttempts)
                    {
                        throw;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            return null;
        }
    }
}
