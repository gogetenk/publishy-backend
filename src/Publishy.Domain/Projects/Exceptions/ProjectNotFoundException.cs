using Publishy.Domain.Common.Exceptions;

namespace Publishy.Domain.Projects.Exceptions;

public class ProjectNotFoundException : DomainException
{
    public ProjectNotFoundException(string projectId) 
        : base($"Project with ID {projectId} not found.")
    {
        ProjectId = projectId;
    }

    public string ProjectId { get; }
}