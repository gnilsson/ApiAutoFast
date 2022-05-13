using ApiAutoFast;
using ApiAutoFast.Sample.Server.Database;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddResponseCaching();
builder.Services.AddFastEndpoints(new[] { typeof(IAssemblyMarker).Assembly });

builder.Services
    .AddSwaggerDoc(s =>
    {
        s.DocumentName = "Initial Release";
        s.Title = "Web API";
        s.Version = "v0.0";
    }, shortSchemaNames: true);

var app = await builder.BuildAutoFastAsync<AutoFastSampleDbContext>("sqlConn");

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
