using Microsoft.Extensions.DependencyInjection;

namespace ApiAutoFast;

internal interface IAutoFastImplementor
{
    void Run();

    IServiceCollection RegisterServices(IServiceCollection services);
}
