using MassTransit;
using Publishy.Application.UseCases.Queries.GetProjects;

namespace Publishy.WebApi;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMassTransit(this IServiceCollection services)
    {
        services.AddMediator(cfg =>
        {
            cfg.AddConsumers(typeof(GetProjectsQuery).Assembly);
            //cfg.ConfigureMediator((context, mcfg) =>
            //{
            //    mcfg.UseSendFilter(typeof(LoggingFilter<>), context);
            //});
        });

        return services;
    }
}

