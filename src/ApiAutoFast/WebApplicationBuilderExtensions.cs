using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiAutoFast;

public static class WebApplicationBuilderExtensions
{
    public static async Task<WebApplication> BuildAutoFastAsync<TContext>(this WebApplicationBuilder builder, string connectionStringConfigurationName) where TContext : DbContext
    {
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
