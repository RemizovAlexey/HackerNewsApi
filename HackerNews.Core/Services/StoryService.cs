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
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public StoryService(IHttpClientFactory httpClientFactory, IMapper mapper, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IEnumerable<Story>> GetBestStories(int count)
        {
            var bestStoryIds = await GetBestStoryIdsWithRetry();
            var stories = await GetStoryDetails(bestStoryIds.Take(count));

            return stories.OrderByDescending(s => s.Score);
        }

        private async Task<IEnumerable<Story>> GetStoryDetails(IEnumerable<int> storyIds)
        {
            var tasks = storyIds.Select(id => GetStoryDetailWithRetry(id));
            var stories = await Task.WhenAll(tasks);
            return stories.Where(s => s != null);
        }

        private async Task<IEnumerable<int>> GetBestStoryIdsWithRetry(int maxAttempts = 3)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var response = await _httpClient.GetAsync(_configuration.GetValue<string>("BestStory:Beststories"));
                    response.EnsureSuccessStatusCode();

                    var contentStream = await response.Content.ReadAsStreamAsync();
                    var bestStoryIds = await JsonSerializer.DeserializeAsync<IEnumerable<int>>(contentStream, Options);

                    return bestStoryIds;
                }
                catch (HttpRequestException ex)
                {
                    // Если это последняя попытка, пробросить исключение дальше
                    if (attempt == maxAttempts)
                    {
                        throw;
                    }

                    // Если это не последняя попытка, вывести сообщение об ошибке и повторить попытку через некоторое время
                    Console.WriteLine($"Failed to retrieve best story IDs on attempt {attempt}. Retrying in 1 second. Error: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            // Недостижимая ветка кода, возвращаем null, если все попытки были неудачными
            return null;
        }

        private async Task<Story> GetStoryDetailWithRetry(int storyId, int maxAttempts = 3)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"{_configuration.GetValue<string>("BestStory:Item")}/{storyId}.json");
                    response.EnsureSuccessStatusCode();

                    using var contentStream = await response.Content.ReadAsStreamAsync();
                    var storyDto = await JsonSerializer.DeserializeAsync<StoryDto>(contentStream, Options);

                    return _mapper.Map<Story>(storyDto);
                }
                catch (HttpRequestException ex)
                {
                    // Если это последняя попытка, пробросить исключение дальше
                    if (attempt == maxAttempts)
                    {
                        throw;
                    }

                    // Если это не последняя попытка, вывести сообщение об ошибке и повторить попытку через некоторое время
                    Console.WriteLine($"Failed to retrieve story with ID {storyId} on attempt {attempt}. Retrying in 1 second. Error: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }

            // Недостижимая ветка кода, возвращаем null, если все попытки были неудачными
            return null;
        }
    }
}
