using ApiAutoFast.Pagination;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.Extensions.DependencyInjection;

namespace ApiAutoFast;

public class AutoFastOptions
{
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutoFast(this IServiceCollection services, Action<AutoFastOptions> setupAction = null!)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (setupAction is not null)
        {
            services.Configure(setupAction);
        }

        services.AddFastEndpoints(new[] { typeof(IAutoFastAssemblyMarker).Assembly });

        services.AddPagination(c =>
        {
            c.PageQueryParameterName = "p";
        });

        return services;
    }
}
