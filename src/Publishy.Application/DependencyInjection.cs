using FluentValidation;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Publishy.Application.Common.Validation;
using System.Reflection;

namespace Publishy.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<>));
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddMediator(cfg =>
        {
            cfg.AddConsumers(assembly);
            cfg.ConfigureMediator((context, mcfg) =>
            {
                mcfg.UseConsumeFilter(typeof(ValidationBehavior<>), context);
            });
        });

        return services;
    }
}