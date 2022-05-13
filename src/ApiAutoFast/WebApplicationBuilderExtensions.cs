using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace ApiAutoFast;

public static class WebApplicationBuilderExtensions
{
    public delegate object EmptyConstructorDelegate();

    public static EmptyConstructorDelegate CreateEmptyConstructor(Type type)
    {
        return Expression.Lambda<EmptyConstructorDelegate>(
            Expression.New(
                type.GetConstructor(Type.EmptyTypes)!)).Compile();
    }

    public static async Task<WebApplication> BuildAutoFastAsync<TContext>(this WebApplicationBuilder builder, string connectionStringConfigurationName) where TContext : DbContext
    {
        var configurationBase = typeof(TContext).Assembly.DefinedTypes.SingleOrDefault(x => x.BaseType == typeof(AutoFastConfiguration));

        if (configurationBase is not null)
        {
            var configuration = (AutoFastConfiguration)CreateEmptyConstructor(configurationBase.UnderlyingSystemType)();

            configuration.Configure();
        }

        builder.Services.AddDbContext<TContext>(options => options
            .UseSqlServer(builder.Configuration.GetConnectionString(connectionStringConfigurationName))
            .LogTo(Console.WriteLine)
            .EnableSensitiveDataLogging());

        builder.Services.AddUriService();

        var app = builder.Build();

        var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        await context.Database.MigrateAsync();

        return app;
    }
}
