using ApiAutoFast.Utility;
using FastEndpoints;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ApiAutoFast;

internal sealed class AutoFastImplementor : IAutoFastImplementor
{
    private readonly AutoFastOptions _options;

    public AutoFastImplementor(AutoFastOptions options)
    {
        _options = options;
    }

    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        services.AddFastEndpoints(new[] { typeof(IAutoFastAssemblyMarker).Assembly });

        services.AddPagination(c =>
        {
            c.PageQueryParameterName = "p";
        });

        return services;
    }

    public void Run()
    {
        var assembly = _options.AssemblyType?.Assembly is not null ? _options.AssemblyType.Assembly : Assembly.GetEntryAssembly();

        if (assembly is null) throw new InvalidOperationException("No specified entry assembly.");

        var types = assembly.GetExportedTypes();

        var configType = types.FirstOrDefault(x => x.BaseType == typeof(AutoFastConfiguration));

        if (configType is not null)
        {
            var ctor = ExpressionUtility.CreateEmptyConstructor(configType);
            var config = ctor() as AutoFastConfiguration;
            config!.Configure();
        }
    }
}
