using System.Net;
using AutoFixture;
using FluentAssertions;
using Publishy.Application.Domain.ValueObjects;
using Publishy.Application.UseCases.Commands.CreateProject;
using Publishy.Application.UseCases.Queries.GetProjects;
using Publishy.IntegrationTests.Fixtures;
using Publishy.IntegrationTests.Helpers;
using Xunit;

namespace Publishy.IntegrationTests.Tests;

public class ProjectEndpointsTests : IClassFixture<MongoDbFixture>
{
    private readonly TestWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly Fixture _fixture;

    public ProjectEndpointsTests(MongoDbFixture mongoDbFixture)
    {
        _factory = new TestWebApplicationFactory(mongoDbFixture);
        _client = _factory.CreateClient();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetProjects_ReturnsEmptyList_WhenNoProjectsExist()
    {
        // Act
        var response = await _client.GetFromJsonAsync<GetProjectsResponse>("/projects");

        // Assert
        response.Should().NotBeNull();
        response!.Data.Should().BeEmpty();
        response.Pagination.TotalItems.Should().Be(0);
    }

    [Fact]
    public async Task CreateProject_ReturnsCreatedProject_WhenDataIsValid()
    {
        // Arrange
        var command = new CreateProjectCommand(
            ProjectName: _fixture.Create<string>(),
            Description: _fixture.Create<string>(),
            Industry: _fixture.Create<string>(),
            Objectives: new[] { "Objective 1", "Objective 2" },
            TargetAudience: new TargetAudience("TestType", "TestDescription"),
            BrandTone: "Professional",
            Website: "https://test.com",
            SocialMedias: new[]
            {
                new SocialMedia("Twitter", 3, "UTC")
            }
        );

        // Act
        var response = await _client.PostAsJsonAsync<ProjectResponse>("/projects", command);

        // Assert
        response.Should().NotBeNull();
        response!.Name.Should().Be(command.ProjectName);
        response.Description.Should().Be(command.Description);
        response.Industry.Should().Be(command.Industry);
        response.Status.Should().Be("Active");
    }

    [Fact]
    public async Task GetProjectById_ReturnsProject_WhenProjectExists()
    {
        // Arrange
        var command = new CreateProjectCommand(
            ProjectName: _fixture.Create<string>(),
            Description: _fixture.Create<string>(),
            Industry: _fixture.Create<string>(),
            Objectives: new[] { "Objective 1" },
            TargetAudience: new TargetAudience("TestType", "TestDescription"),
            BrandTone: "Professional",
            Website: "https://test.com",
            SocialMedias: new[]
            {
                new SocialMedia("Twitter", 3, "UTC")
            }
        );

        var createdProject = await _client.PostAsJsonAsync<ProjectResponse>("/projects", command);

        // Act
        var response = await _client.GetFromJsonAsync<ProjectResponse>($"/projects/{createdProject!.Id}");

        // Assert
        response.Should().NotBeNull();
        response!.Id.Should().Be(createdProject.Id);
        response.Name.Should().Be(command.ProjectName);
    }

    [Fact]
    public async Task GetProjectById_ReturnsNotFound_WhenProjectDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync($"/projects/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetActiveProjects_ReturnsOnlyActiveProjects()
    {
        // Arrange
        var command = new CreateProjectCommand(
            ProjectName: _fixture.Create<string>(),
            Description: _fixture.Create<string>(),
            Industry: _fixture.Create<string>(),
            Objectives: new[] { "Objective 1" },
            TargetAudience: new TargetAudience("TestType", "TestDescription"),
            BrandTone: "Professional",
            Website: "https://test.com",
            SocialMedias: new[]
            {
                new SocialMedia("Twitter", 3, "UTC")
            }
        );

        var project = await _client.PostAsJsonAsync<ProjectResponse>("/projects", command);

        // Act
        var response = await _client.GetFromJsonAsync<ProjectResponse[]>("/projects/active");

        // Assert
        response.Should().NotBeNull();
        response!.Should().ContainSingle();
        response.First().Id.Should().Be(project!.Id);
    }
}