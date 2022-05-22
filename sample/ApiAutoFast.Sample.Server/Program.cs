using ApiAutoFast;
using ApiAutoFast.Sample.Server;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MR.AspNetCore.Pagination;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.AddResponseCaching();
builder.Services.AddFastEndpoints(new[] { typeof(IAutoFastAssemblyMarker).Assembly });






//var app = await builder.BuildAutoFastAsync<AutoFastSampleDbContext>("sqlConn");

builder.Services.AddPagination(c =>
{
    c.PageQueryParameterName = "p";
});

builder.Services
.AddSwaggerDoc(s =>
{
    s.DocumentName = "Initial Release";
    s.Title = "Web API";
    s.Version = "v0.0";
    s.OperationProcessors.Add(new PaginationOperationProcessor());
    //s.AddOperationFilter(x =>
    //{
    //    x.Parameters.A
    //});
    //s.AddOperationFilter(x => new PaginationOperationFilter());
}, shortSchemaNames: true);


//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
//    c.CustomSchemaIds(x => x.FullName);

//    c.TagActionsBy(api => new[] { api.RelativePath });

//    c.ConfigurePagination();
//});

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
