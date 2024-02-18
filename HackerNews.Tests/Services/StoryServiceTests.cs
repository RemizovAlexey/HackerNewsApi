using AutoMapper;
using HackerNews.Core.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using Xunit;

namespace HackerNews.Tests.Services
{
    public class StoryServiceTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly IConfiguration _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "BestStory:Beststories", "https://hacker-news.firebaseio.com/v0/beststories.json" },
                    { "BestStory:Item", "https://hacker-news.firebaseio.com/v0/item" }
                })
                .Build();

        public StoryServiceTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _mapperMock = new Mock<IMapper>();
            _configurationMock = new Mock<IConfiguration>();
        }


        [Fact]
        public async Task GetBestStoryIdsWithRetry_Positive()
        {
            // Arrange
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[1, 2, 3]")
            };

            httpMessageHandlerMock
              .Protected()
              .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
              .ReturnsAsync(response);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);

            _httpClientFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

            var storyService = new StoryService(_httpClientFactoryMock.Object, _mapperMock.Object, _configuration);

            // Act
            var storyIds = await storyService.GetBestStoryIdsWithRetryAsync(3);

            // Assert
            Assert.NotEmpty(storyIds);
            Assert.Equal(3, storyIds.Count());
            Assert.Equal(new List<int> { 1, 2, 3 }, storyIds);
        }

        [Fact]
        public async Task GetBestStoryIdsWithRetry_Negative_HttpRequestException()
        {
            // Arrange
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            httpMessageHandlerMock
              .Protected()
              .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
              .ThrowsAsync(new HttpRequestException("Connection failed"));

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);

            _httpClientFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var storyService = new StoryService(_httpClientFactoryMock.Object, _mapperMock.Object, _configuration);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => storyService.GetBestStoryIdsWithRetryAsync(3));
        }

        [Fact]
        public async Task GetStoryDetails_Negative_EmptyStoryIds()
        {
            // Arrange
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[]")
            };

            httpMessageHandlerMock
              .Protected()
              .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
              .ReturnsAsync(response);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);

            _httpClientFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var storyService = new StoryService(_httpClientFactoryMock.Object, _mapperMock.Object, _configuration);

            // Act
            var storiyIds = await storyService.GetBestStoryIdsWithRetryAsync(3);

            // Assert
            Assert.Empty(storiyIds);
        }

        [Fact]
        public async Task GetBestStoryIdsWithRetry_ThrowsExceptionTwice_Positive()
        {
            // Arrange
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[1, 2, 3]")
            };

            httpMessageHandlerMock
              .Protected()
              .SetupSequence<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
              .ThrowsAsync(new HttpRequestException("Connection failed"))
              .ThrowsAsync(new HttpRequestException("Connection failed"))
              .ReturnsAsync(response);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);

            _httpClientFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var storyService = new StoryService(_httpClientFactoryMock.Object, _mapperMock.Object, _configuration);

            // Act
            var storyIds = await storyService.GetBestStoryIdsWithRetryAsync();

            //// Assert
            Assert.NotNull(storyIds);
            Assert.Equal(new List<int> { 1, 2, 3 }, storyIds);
        }

        [Fact]
        public async Task GetBestStoryIdsWithRetry_Negative_AllAttemptsFailed()
        {
            // Arrange
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[1, 2, 3]")
            };

            httpMessageHandlerMock
              .Protected()
              .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
              .ThrowsAsync(new HttpRequestException("Connection failed"));

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);

            _httpClientFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            var storyService = new StoryService(_httpClientFactoryMock.Object, _mapperMock.Object, _configuration);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => storyService.GetBestStoryIdsWithRetryAsync());
        }

        // TODO: Add unit tests for GetStoryDetails
    }
}
