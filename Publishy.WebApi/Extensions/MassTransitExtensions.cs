using MassTransit;
using Publishy.Api.Endpoints;

namespace CryptoCard.ApiService.MassTransit;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMassTransit(this IServiceCollection services)
    {
        services.AddMediator(cfg =>
        {
            cfg.AddConsumers(typeof(ProjectEndpoints).Assembly);
            //cfg.ConfigureMediator((context, mcfg) =>
            //{
            //    mcfg.UseSendFilter(typeof(LoggingFilter<>), context);
            //});
        });

        return services;
    }
}

