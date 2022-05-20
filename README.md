# ApiAutoFast
### Fast and automagical, pristine API

**ApiAutoFast** is a library that aims to construct a REST api around your values while still maintaining the nessescary flexibility for your logic!
To be able to accomplish this, we need to stand on the shoulders of giants; taking dependencies on three well known libraries:
*FastEndpoints*, *Mapster* and *EntityFramework*.

### Required packages

```console
dotnet add package ApiAutoFast --version 0.4.1-beta
dotnet add package ApiAutoFast.SourceGenerator --version 0.4.1-beta
dotnet add package FastEndpoints --version 4.1.0
dotnet add package FastEndpoints.Swagger --version 4.1.0
dotnet add package Mapster --version 7.3.0
dotnet add package Microsoft.EntityFrameworkCore --version 6.0.5
dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.5
```

### The setup

```C#

[AutoFastEndpoints]
public sealed class AuthorEntity
{
    public Name Name { get; set; } = default!;
    public BlogsRelation Blogs { get; set; } = default!;
}

[AutoFastEndpoints]
public sealed class BlogEntity
{
    public Title Title { get; set; } = default!;
    public BlogCategory Category { get; set; } = default!;
    public FirstLikeDateTime FirstLikeDateTime { get; set; } = default!;
}

[AutoFastContext]
public partial class DemoAafContext
{

}

```

This setup will yield crud endpoints for each entity, with the availability to extend each generated partial endpoint and modify the pipeline.

The properties are derived from a base class called DomainValue.

```C#

public sealed class Title : DomainValue<string, Title>
{
    private const string RegexPattern = "";

    protected override bool TryValidateRequestConversion(string? requestValue, out string entityValue)
    {
        entityValue = requestValue!;
        return requestValue is not null && Regex.IsMatch(requestValue, RegexPattern);
    }

    protected override string? MessageOnFailedValidation => "Incorrect format on Title.";
}

public sealed class BlogCategory : DomainValue<EBlogCategory, EBlogCategory, string, BlogCategory>
{ }

public sealed class Name : DomainValue<string, Name>
{ }

public sealed class BlogsRelation : DomainValue<ICollection<Blog>, BlogsRelation>
{ }

public sealed class FirstLikeDateTime : DomainValue<string, DateTime, FirstLikeDateTime>
{
    protected override bool TryValidateRequestConversion(string? requestValue, out DateTime entityValue)
    {
        entityValue = default!;
        return requestValue is not null && DateTime.TryParse(requestValue, out entityValue);
    }

    public override string ToString() => EntityValue.ToLongDateString();
}

```

And it takes generic parameters to define the state of the property across the domain, in order: TRequest, TEntity, TResponse.

As well have a method TryValidateRequestConversion that is going to execute on CreateEndpoint.

A start up can look like this:

```C#

global using ApiAutoFast;
global using FastEndpoints;
using DemoAaf;
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

var app = await builder.BuildAutoFastAsync<DemoAafContext>("sqlConn");

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

```

It is also important to not forget the required mapster setup

```console
#skip this step if you already have dotnet-tools.json
dotnet new tool-manifest 

dotnet tool install Mapster.Tool
```

And finally we configure our .csproj file such that we have a command to generate our mapster files (all other files are generated incrementally).

```xml

<Target Name="Mapster">
  <Exec WorkingDirectory="$(ProjectDir)" Command="dotnet msbuild -p:IgnoreFolder=$(IgnoreFolder)" />
  <Exec WorkingDirectory="$(ProjectDir)" Command="dotnet tool restore" />
  <Exec WorkingDirectory="$(ProjectDir)" Command="dotnet mapster model -a &quot;$(TargetDir)$(ProjectName).dll&quot;" />
  <Exec WorkingDirectory="$(ProjectDir)" Command="dotnet mapster extension -a &quot;$(TargetDir)$(ProjectName).dll&quot;" />
  <Exec WorkingDirectory="$(ProjectDir)" Command="dotnet mapster mapper -a &quot;$(TargetDir)$(ProjectName).dll&quot;" />
</Target>

<ItemGroup>
  <Generated Include="**\*.g.cs" />
</ItemGroup>

<Target Name="CleanGenerated">
  <Delete Files="@(Generated)" />
</Target>

<Target Name="AutoFastMap">
  <Exec WorkingDirectory="$(ProjectDir)" Command="dotnet msbuild -t:Mapster -p:IgnoreFolder=$(IgnoreFolder)" />
  <Exec WorkingDirectory="$(ProjectDir)" Command="dotnet msbuild -t:Mapster -p:IgnoreFolder=$(IgnoreFolder)" />
</Target>

<PropertyGroup>
  <IgnoreFolder>false</IgnoreFolder>
  <FolderToIgnore>Endpoints/**/*.cs</FolderToIgnore>
  <ProjectToInclude>../$(ProjectName).csproj/*/*.cs</ProjectToInclude>
</PropertyGroup>

<ItemGroup>
  <Compile Condition="$(IgnoreFolder) == true" Include="$(ProjectToInclude)"/>
  <Compile Condition="$(IgnoreFolder) == true" Remove="$(FolderToIgnore)" />
  <Content Condition="$(IgnoreFolder) == true" Include="$(FolderToIgnore)" />
</ItemGroup>

```

To generate the mapster files use
```console
dotnet msbuild -t:AutoFastMap
```
And to remove
```console
dotnet msbuild -t:CleanGenerated
```
And if you have extended some partial endpoint with your logic this config presumes you've put them in a Endpoints folder.
If you have made a change to a entity, e.g removed a property, there will be a compilation error in the mapping files, and then you can use the clean command.
However after cleaning, your custom endpoints will get an error. And then you can use
```console
dotnet msbuild -t:AutoFastMap -p:IgnoreFolder=true
```
And that will generate mapster while ignoring the errors under /Endpoints!


😃
