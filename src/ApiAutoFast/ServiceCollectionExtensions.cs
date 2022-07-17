using ApiAutoFast.Utility;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;

namespace ApiAutoFast;

public class AutoFastOptions
{

}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAutoFast(this IServiceCollection services, Type assembly, Action<AutoFastOptions> setupAction = null!)
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (setupAction is not null)
        {
            services.Configure(setupAction);
        }

        var types = assembly.Assembly.GetTypes();

        var configType = types.FirstOrDefault(x => x.BaseType == typeof(AutoFastConfiguration));

        if (configType is not null)
        {
            var ctor = ExpressionUtility.CreateEmptyConstructor(configType);
            var config = ctor() as AutoFastConfiguration;
            config!.Configure();
        }

        services.AddFastEndpoints(new[] { typeof(IAutoFastAssemblyMarker).Assembly });

        services.AddPagination(c =>
        {
            c.PageQueryParameterName = "p";
        });

        return services;
    }
}
