using AutoFixture;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.UseCases.Commands.CreatePost;
using Publishy.Application.UseCases.Commands.CreateProject;
using Publishy.IntegrationTests.Helpers;

namespace Publishy.IntegrationTests.Tests.Posts.Helpers;

public class PostTestHelper
{
    private readonly HttpClient _client;
    private readonly Fixture _fixture;

    public PostTestHelper(HttpClient client, Fixture fixture)
    {
        _client = client;
        _fixture = fixture;
    }

    public async Task<ProjectResponse> CreateProjectAsync()
    {
        var command = new CreateProjectCommand(
            ProjectName: _fixture.Create<string>(),
            Description: _fixture.Create<string>(),
            Industry: _fixture.Create<string>(),
            Objectives: new[] { "Test Objective" },
            TargetAudience: new TargetAudience("TestType", "TestDescription"),
            BrandTone: "Professional",
            Website: "https://test.com",
            SocialMedias: new[] { new SocialMedia("Twitter", 3, "UTC") }
        );

        return await _client.PostAsJsonAsync<ProjectResponse>("/projects", command);
    }

    public CreatePostCommand CreatePostCommand(string projectId)
    {
        return new CreatePostCommand(
            ProjectId: projectId,
            Title: _fixture.Create<string>(),
            Content: _fixture.Create<string>(),
            Platform: "Twitter",
            ScheduledFor: null,
            Tags: new List<string> { "test" },
            MediaAssets: new List<MediaAssetDto>()
        );
    }

    public async Task<PostResponse> CreatePostAsync(string projectId)
    {
        var command = CreatePostCommand(projectId);
        return await _client.PostAsJsonAsync<PostResponse>("/posts", command);
    }
}