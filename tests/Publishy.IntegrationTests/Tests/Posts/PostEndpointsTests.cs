using AutoFixture;
using FluentAssertions;
using Publishy.Application.UseCases.Commands.CreatePost;
using Publishy.Application.UseCases.Queries.GetPosts;
using Publishy.IntegrationTests.Fixtures;
using Publishy.IntegrationTests.Helpers;
using Publishy.IntegrationTests.Tests.Posts.Helpers;
using System.Net;
using Xunit;

namespace Publishy.IntegrationTests.Tests.Posts;

public class PostEndpointsTests : IClassFixture<MongoDbFixture>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly Fixture _fixture;
    private readonly PostTestHelper _postHelper;

    public PostEndpointsTests(MongoDbFixture mongoDbFixture)
    {
        _factory = new TestWebApplicationFactory(mongoDbFixture);
        _client = _factory.CreateClient();
        _fixture = new Fixture();
        _postHelper = new PostTestHelper(_client, _fixture);
    }

    [Fact]
    public async Task GetPosts_ReturnsEmptyList_WhenNoPostsExist()
    {
        // Act
        var response = await _client.GetFromJsonAsync<GetPostsResponse>("/posts");

        // Assert
        response.Should().NotBeNull();
        response!.Data.Should().BeEmpty();
        response.Pagination.TotalItems.Should().Be(0);
    }

    [Fact]
    public async Task GetPosts_ReturnsFilteredPosts_WhenFilterParametersProvided()
    {
        // Arrange
        var project = await _postHelper.CreateProjectAsync();
        var post = await _postHelper.CreatePostAsync(project.Id);

        // Act
        var response = await _client.GetFromJsonAsync<GetPostsResponse>($"/posts?projectId={project.Id}&status=Draft&platform={post.Platform}");

        // Assert
        response.Should().NotBeNull();
        response!.Data.Should().ContainSingle();
        response.Data.First().Id.Should().Be(post.Id);
    }

    [Fact]
    public async Task CreatePost_ReturnsCreatedPost_WhenDataIsValid()
    {
        // Arrange
        var project = await _postHelper.CreateProjectAsync();
        var command = _postHelper.CreatePostCommand(project.Id);

        // Act
        var response = await _client.PostAsJsonAsync<PostResponse>("/posts", command);

        // Assert
        response.Should().NotBeNull();
        response!.Title.Should().Be(command.Title);
        response.Content.Should().Be(command.Content);
        response.Platform.Should().Be(command.Platform);
        response.Status.Should().Be("Draft");
    }

    [Fact]
    public async Task GetPostById_ReturnsPost_WhenPostExists()
    {
        // Arrange
        var project = await _postHelper.CreateProjectAsync();
        var createdPost = await _postHelper.CreatePostAsync(project.Id);

        // Act
        var response = await _client.GetFromJsonAsync<PostResponse>($"/posts/{createdPost.Id}");

        // Assert
        response.Should().NotBeNull();
        response!.Id.Should().Be(createdPost.Id);
        response.Title.Should().Be(createdPost.Title);
    }

    [Fact]
    public async Task GetPostById_ReturnsNotFound_WhenPostDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync($"/posts/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}