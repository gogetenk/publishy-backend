using System.Net;
using AutoFixture;
using FluentAssertions;
using Publishy.Application.UseCases.Commands.CreatePost;
using Publishy.Application.UseCases.Commands.UpdatePost;
using Publishy.IntegrationTests.Fixtures;
using Publishy.IntegrationTests.Helpers;
using Publishy.IntegrationTests.Tests.Posts.Helpers;

namespace Publishy.IntegrationTests.Tests.Posts;

public class PostUpdateTests : IClassFixture<MongoDbFixture>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly Fixture _fixture;
    private readonly PostTestHelper _postHelper;

    public PostUpdateTests(MongoDbFixture mongoDbFixture)
    {
        _factory = new TestWebApplicationFactory(mongoDbFixture);
        _client = _factory.CreateClient();
        _fixture = new Fixture();
        _postHelper = new PostTestHelper(_client, _fixture);
    }

    [Fact]
    public async Task UpdatePost_UpdatesPostDetails_WhenDataIsValid()
    {
        // Arrange
        var project = await _postHelper.CreateProjectAsync();
        var post = await _postHelper.CreatePostAsync(project.Id);
        var command = new UpdatePostCommand(
            PostId: post.Id,
            Title: _fixture.Create<string>(),
            Content: _fixture.Create<string>(),
            Platform: "Twitter",
            ScheduledFor: null,
            Tags: new List<string> { "updated" },
            MediaAssets: new List<MediaAssetDto>()
        );

        // Act
        var response = await _client.PutAsJsonAsync<PostResponse>($"/posts/{post.Id}", command);

        // Assert
        response.Should().NotBeNull();
        response!.Title.Should().Be(command.Title);
        response.Content.Should().Be(command.Content);
        response.Tags.Should().BeEquivalentTo(command.Tags);
    }

    [Fact]
    public async Task UpdatePost_ReturnsBadRequest_WhenPostIsPublished()
    {
        // Arrange
        var project = await _postHelper.CreateProjectAsync();
        var post = await _postHelper.CreatePostAsync(project.Id);
        await _client.PostAsJsonAsync<PostResponse>($"/posts/{post.Id}/publish", new { });

        var command = new UpdatePostCommand(
            PostId: post.Id,
            Title: _fixture.Create<string>(),
            Content: _fixture.Create<string>(),
            Platform: "Twitter",
            ScheduledFor: null,
            Tags: new List<string>(),
            MediaAssets: new List<MediaAssetDto>()
        );

        // Act
        var response = await _client.PutAsync($"/posts/{post.Id}", JsonContent.Create(command));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeletePost_RemovesPost_WhenPostExists()
    {
        // Arrange
        var project = await _postHelper.CreateProjectAsync();
        var post = await _postHelper.CreatePostAsync(project.Id);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/posts/{post.Id}");
        var getResponse = await _client.GetAsync($"/posts/{post.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}