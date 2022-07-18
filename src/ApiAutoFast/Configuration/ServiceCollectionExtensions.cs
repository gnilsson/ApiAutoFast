using Microsoft.Extensions.DependencyInjection;

namespace ApiAutoFast;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutoFast(this IServiceCollection services, Action<AutoFastOptions>? setupAction = null)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (setupAction is not null)
        {
            services.Configure(setupAction);
        }

        var options = new AutoFastOptions();

        setupAction?.Invoke(options);

        var impl = new AutoFastImplementor(options);

        impl.Run();

        services = impl.RegisterServices(services);

        return services;
    }
}
