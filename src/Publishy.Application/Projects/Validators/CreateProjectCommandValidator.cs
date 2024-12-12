using FluentValidation;
using Publishy.Api.Modules.Projects.Commands;

namespace Publishy.Application.Projects.Validators;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.ProjectName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Industry)
            .NotEmpty();

        RuleFor(x => x.Objectives)
            .NotEmpty();

        RuleFor(x => x.TargetAudience.Type)
            .NotEmpty()
            .Must(type => type is "B2C" or "B2B")
            .WithMessage("Target audience type must be either B2C or B2B");

        RuleFor(x => x.Website)
            .NotEmpty()
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Website must be a valid URL");

        RuleFor(x => x.SocialMedias)
            .NotEmpty()
            .Must(sm => sm.Length <= 5)
            .WithMessage("A maximum of 5 social media platforms can be configured");
    }
}