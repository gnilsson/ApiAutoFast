global using ApiAutoFast;
using ApiAutoFast.Pagination;
using ApiAutoFast.Sample.Server;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddResponseCaching();

builder.Services
    .AddSwaggerDoc(s =>
    {
        s.DocumentName = "Initial Release";
        s.Title = "Web API";
        s.Version = "v0.0";
        s.OperationProcessors.Add(new KeysetPaginationOperationProcessor());
    }, shortSchemaNames: true);

builder.Services.AddAutoFast(typeof(Program));

builder.Services.AddDbContext<AutoFastSampleDbContext>(options => options
    .UseSqlServer(builder.Configuration.GetConnectionString("sqlConn"))
    .LogTo(Console.WriteLine)
    .EnableSensitiveDataLogging());

var app = builder.Build();

var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<AutoFastSampleDbContext>();
await context.Database.MigrateAsync();

app.UseDefaultExceptionHandler();

app.UseResponseCaching();

app.UseRouting();

app.UseCors(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseAuthorization();

app.UseFastEndpoints(config =>
{
    config.RoutingOptions = o => o.Prefix = "api";
    config.VersioningOptions = o => o.Prefix = "v";
});

if (!app.Environment.IsProduction())
{
    app.UseOpenApi();
    app.UseSwaggerUi3(s => s.ConfigureDefaults());
}

app.Run();
