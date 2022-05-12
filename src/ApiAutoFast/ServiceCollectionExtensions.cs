using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ApiAutoFast;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddUriService(this IServiceCollection services)
    {
        return services.AddScoped<IUriService>(provider =>
        {
            var accessor = provider.GetRequiredService<IHttpContextAccessor>();
            var request = accessor.HttpContext!.Request;
            var absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), "/");
            return new UriService(absoluteUri);
        });
    }
}
