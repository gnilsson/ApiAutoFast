# ApiAutoFast
### Fast and automagical, pristine API

**ApiAutoFast** is a library that aims to construct a REST api around your values while still maintaining the nessescary flexibility for your logic!
To be able to accomplish this, we need to stand on the shoulders of giants; taking dependencies on three well known libraries:
*FastEndpoints*, *Mapster* and *EntityFramework*.

### Required packages

```console
dotnet add package ApiAutoFast --version 0.5.0-beta
dotnet add package ApiAutoFast.SourceGenerator --version 0.5.1-beta
dotnet add package FastEndpoints.Swagger --version 4.2.0
dotnet add package Mapster --version 7.3.0
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
    public PublishedDateTime PublishedDateTime { get; set; } = default!;
}

[AutoFastContext]
public partial class DemoAafContext
{
    partial void ExtendOnModelCreating(ModelBuilder modelBuilder)
    {
        // use if needed!
        // other methods that exist in DbContext are fine to override.
    }
}
```

This setup will yield crud endpoints for each entity, with the availability to extend each generated partial endpoint and modify the pipeline.

The properties are derived from a base class called DomainValue.

(Currently the namespace in which you define your entities must be the same as the one in which you define partial classes).

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

public sealed class BlogsRelation : DomainValue<ICollection<Blog>, BlogsRelation>
{ }

public sealed class PublishedDateTime : DomainValue<string, DateTime, PublishedDateTime>
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

### Startup

Configure fastendpoints swagger with pagination parameters:
```C#
builder.Services
    .AddSwaggerDoc(s =>
    {
        s.DocumentName = "Initial Release";
        s.Title = "Web API";
        s.Version = "v0.0";
        s.OperationProcessors.Add(new KeysetPaginationOperationProcessor());
    }, shortSchemaNames: true);
```

```C#
builder.Services.AddAutoFast();
```

Usual ef core setup, requiring migrations:
```C#
builder.Services.AddDbContext<AutoFastSampleDbContext>(options => options
    .UseSqlServer(builder.Configuration.GetConnectionString("sqlConn"))
    .LogTo(Console.WriteLine)
    .EnableSensitiveDataLogging());
```

Register endpoints discovery with FastEndpoints:
```C#
app.UseFastEndpoints(config =>
{
    config.RoutingOptions = o => o.Prefix = "api";
    config.VersioningOptions = o => o.Prefix = "v";
});
```

NSwag swagger registration:
```C#
if (!app.Environment.IsProduction())
{
    app.UseOpenApi();
    app.UseSwaggerUi3(s => s.ConfigureDefaults());
}
```

It is also important to not forget the required mapster setup

```console
#skip this step if you already have dotnet-tools.json
dotnet new tool-manifest 

dotnet tool install Mapster.Tool
```

And finally we configure our .csproj file such that we have a command to generate our mapster files (all other files are generated incrementally).

```xml
	<PropertyGroup>
		<!-- 👇 Persist the source generator (and other) files to disk -->
		<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
		<!-- 👇 The "base" path for the source generators -->
		<GeneratedFolder>Generated</GeneratedFolder>
		<!-- 👇 Write the output for each target framework to a different sub-folder -->
		<CompilerGeneratedFilesOutputPath>$(GeneratedFolder)\$(TargetFramework)</CompilerGeneratedFilesOutputPath>
	</PropertyGroup>

	<ItemGroup>
		<!-- 👇 Exclude from compilation everything in the base folder -->
		<Compile Remove="$(GeneratedFolder)/**/*.cs" />
		<!-- 👇 Keep in project as content -->
		<Content Include="$(GeneratedFolder)/**/*.cs" />
	</ItemGroup>

	<!-- Mapster generation commands -->
	<Target Name="Mapster">
		<Exec WorkingDirectory="$(ProjectDir)" Command="dotnet msbuild -p:IgnoreFolder=$(IgnoreFolder)" />
		<Exec WorkingDirectory="$(ProjectDir)" Command="dotnet tool restore" />
		<Exec WorkingDirectory="$(ProjectDir)" Command="dotnet mapster model -a &quot;$(TargetDir)$(ProjectName).dll&quot;" />
		<Exec WorkingDirectory="$(ProjectDir)" Command="dotnet mapster extension -a &quot;$(TargetDir)$(ProjectName).dll&quot;" />
		<Exec WorkingDirectory="$(ProjectDir)" Command="dotnet mapster mapper -a &quot;$(TargetDir)$(ProjectName).dll&quot;" />
	</Target>

	<!-- Mapster needs two builds to fully generate mapper class -->
	<Target Name="AutoFastMap">
		<Exec WorkingDirectory="$(ProjectDir)" Command="dotnet msbuild -t:Mapster -p:IgnoreFolder=$(IgnoreFolder)" />
		<Exec WorkingDirectory="$(ProjectDir)" Command="dotnet msbuild -t:Mapster -p:IgnoreFolder=$(IgnoreFolder)" />
	</Target>

	<PropertyGroup>
		<!-- 👇 Flag to determine to exclude a folder from build -->
		<IgnoreFolder>false</IgnoreFolder>
		<!-- 👇 A folder that has errors before everything is generated -->
		<FolderToIgnore>Endpoints/**/*.cs</FolderToIgnore>
		<!-- 👇 Ensure our project is part of build to trigger generator -->
		<ProjectToInclude>../$(ProjectName).csproj/*/*.cs</ProjectToInclude>
	</PropertyGroup>


	<!-- Parameter passed to build around files that depend on generated files -->
	<ItemGroup>
		<Compile Condition="$(IgnoreFolder) == true" Include="$(ProjectToInclude)" />
		<Compile Condition="$(IgnoreFolder) == true" Remove="$(FolderToIgnore)" />
		<Content Condition="$(IgnoreFolder) == true" Include="$(FolderToIgnore)" />
	</ItemGroup>

	<ItemGroup>
		<!-- 👇 All generated files -->
		<Generated Include="**\*.g.cs" />
	</ItemGroup>

	<!-- Clean generated files, needed for mapster files -->
	<Target Name="CleanGenerated">
		<Delete Files="@(Generated)" />
	</Target>
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
