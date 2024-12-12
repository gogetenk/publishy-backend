using FluentValidation;
using MassTransit;

namespace Publishy.Application.Common.Validation;

public class ValidationBehavior<TMessage> : IFilter<ConsumeContext<TMessage>>
    where TMessage : class
{
    private readonly IValidator<TMessage>[] _validators;

    public ValidationBehavior(IEnumerable<IValidator<TMessage>> validators)
    {
        _validators = validators.ToArray();
    }

    public async Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
    {
        if (_validators.Any())
        {
            var validationContext = new ValidationContext<TMessage>(context.Message);
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(validationContext)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("validation");
    }
}