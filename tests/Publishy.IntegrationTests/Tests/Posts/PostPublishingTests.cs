using System.Net;
using AutoFixture;
using FluentAssertions;
using Publishy.Application.UseCases.Commands.CreatePost;
using Publishy.IntegrationTests.Fixtures;
using Publishy.IntegrationTests.Helpers;
using Publishy.IntegrationTests.Tests.Posts.Helpers;

namespace Publishy.IntegrationTests.Tests.Posts;

public class PostPublishingTests : IClassFixture<MongoDbFixture>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly Fixture _fixture;
    private readonly PostTestHelper _postHelper;

    public PostPublishingTests(MongoDbFixture mongoDbFixture)
    {
        _factory = new TestWebApplicationFactory(mongoDbFixture);
        _client = _factory.CreateClient();
        _fixture = new Fixture();
        _postHelper = new PostTestHelper(_client, _fixture);
    }

    [Fact]
    public async Task PublishPost_UpdatesPostStatus_WhenPostIsDraft()
    {
        // Arrange
        var project = await _postHelper.CreateProjectAsync();
        var post = await _postHelper.CreatePostAsync(project.Id);

        // Act
        var response = await _client.PostAsJsonAsync<PostResponse>($"/posts/{post.Id}/publish", new { });

        // Assert
        response.Should().NotBeNull();
        response!.Status.Should().Be("Published");
        response.PublishedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task SchedulePost_UpdatesScheduledDate_WhenDateIsValid()
    {
        // Arrange
        var project = await _postHelper.CreateProjectAsync();
        var post = await _postHelper.CreatePostAsync(project.Id);
        var scheduledDate = DateTime.UtcNow.AddDays(1);

        // Act
        var response = await _client.PostAsJsonAsync<PostResponse>($"/posts/{post.Id}/schedule", scheduledDate);

        // Assert
        response.Should().NotBeNull();
        response!.Status.Should().Be("Scheduled");
        response.ScheduledFor.Should().Be(scheduledDate);
    }

    [Fact]
    public async Task CancelPost_ReturnsToDraftStatus_WhenPostIsScheduled()
    {
        // Arrange
        var project = await _postHelper.CreateProjectAsync();
        var post = await _postHelper.CreatePostAsync(project.Id);
        await _client.PostAsJsonAsync<PostResponse>($"/posts/{post.Id}/schedule", DateTime.UtcNow.AddDays(1));

        // Act
        var response = await _client.PostAsJsonAsync<PostResponse>($"/posts/{post.Id}/cancel", new { });

        // Assert
        response.Should().NotBeNull();
        response!.Status.Should().Be("Draft");
        response.ScheduledFor.Should().BeNull();
    }
}