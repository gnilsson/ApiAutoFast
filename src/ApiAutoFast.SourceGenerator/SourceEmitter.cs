﻿using System.Text;

namespace ApiAutoFast.SourceGenerator;

public static class SourceEmitter
{
    public const string AutoFastEndpointsAttribute = @"
namespace ApiAutoFast;

/// <summary>
/// Marker attribute for source generator.
/// <param name=""entityName"">Name of the entity to generate, will default to this class name and remove ""Config""</param>
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class)]
internal class AutoFastEndpointsAttribute : System.Attribute
{
    internal AutoFastEndpointsAttribute(string? entityName = null)
    {
        EntityName = entityName;
    }

    public string? EntityName { get; }
}
";

    public const string AutoFastContextAttribute = @"
namespace ApiAutoFast;

[System.AttributeUsage(System.AttributeTargets.Class)]
internal class AutoFastContextAttribute : System.Attribute
{
}
";

    private static readonly Func<string, string> _getModelTargetSource = static (modelTarget) => modelTarget switch
    {
        nameof(AttributeModelTargetType.CreateCommand) => string.Empty,
        nameof(AttributeModelTargetType.QueryRequest) => @"
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }",
        _ => @"
    public string Id { get; set; }",
    };

    private static readonly Func<EndpointTargetType, Func<StringBuilder, EndpointConfig, StringBuilder>> _endpointSourceGets = static (target) => target switch
    {
        EndpointTargetType.Get => static (sb, endpointConfig) =>
        {
            sb.Append(@"
        var result = await _dbContext.").Append(endpointConfig.EntityName).Append(@"s.Where(x => true).ToArrayAsync(ct);

        if (result.Length == 0)
        {
            await SendNotFoundAsync(ct);

            return;
        }

        var response = result.Select(x => Map.FromEntity(x));

        var pag = new ").Append(endpointConfig.Response).Append(@" { Data = response };

        await SendOkAsync(pag, ct);");
            return sb;
        }
        ,
        EndpointTargetType.Create => static (sb, endpointConfig) =>
        {
            sb.Append(@"
        var entity = Map.ToEntity(req);

        await _dbContext.AddAsync(entity, ct);

        await _dbContext.SaveChangesAsync(ct);

        var response = Map.FromEntity(entity);

        await SendCreatedAtAsync<GetById").Append(endpointConfig.EntityName).Append(@"Endpoint>(response.Id, response, Http.GET, cancellation: ct);");
            return sb;
        }
        ,
        EndpointTargetType.Update => static (sb, endpointConfig) =>
        {
            sb.Append(@"
        var result = await _dbContext.").Append(endpointConfig.EntityName).Append(@"s.FindAsync((Identifier)req.Id, ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
        }

        var entity = Map.UpdateEntity(result, req);

        var response = Map.FromEntity(entity);

        await SendOkAsync(response, ct);");
            return sb;
        }
        ,
        EndpointTargetType.Delete => static (sb, endpointConfig) =>
        {
            sb.Append(@"
        var result = await _dbContext.").Append(endpointConfig.EntityName).Append(@"s.FindAsync((Identifier)req.Id, ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
        }

        _dbContext.").Append(endpointConfig.EntityName).Append(@"s.Remove(result);

        await _dbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);");
            return sb;
        }
        ,
        EndpointTargetType.GetById => static (sb, endpointConfig) =>
        {
            sb.Append(@"
        var result = await _dbContext.").Append(endpointConfig.EntityName).Append(@"s.FindAsync((Identifier)req.Id, ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
        }

        var response = Map.FromEntity(result);

        await SendOkAsync(response, ct);");
            return sb;
        }
        ,
        _ => static (sb, _) =>
        {
            sb.Append(@"
        await base.HandleAsync(req, ct);");
            return sb;
        }
    };


    internal static string EmitMappingRegister(StringBuilder sb, EntityGenerationConfig generationConfig)
    {
        sb.Clear();

        sb.Append(@"
using Mapster;
using ApiAutoFast;

namespace ").Append(generationConfig.Namespace).Append(@";

public partial class MappingRegister : ICodeGenerationRegister
{
    private bool _overrideRegisterResponses = false;
    private bool _extendRegisterResponses = false;

    static partial void OnOverrideRegisterResponses(AdaptAttributeBuilder aab);
    static partial void OnExtendRegisterResponses(AdaptAttributeBuilder aab);
    static partial void ExtendRegister(CodeGenerationConfig config);
    static partial void RegisterMappers(CodeGenerationConfig config);

    public void Register(CodeGenerationConfig config)
    {
        var aab = config.AdaptTo(""[name]Response"");

        if (_overrideRegisterResponses)
        {
            OnOverrideRegisterResponses(aab);
        }
        else if (_extendRegisterResponses)
        {
            aab.ForTypeDefaultValues();

            OnExtendRegisterResponses(aab);
        }
        else
        {
            aab.ForTypeDefaultValues();
        }

        TypeAdapterConfig.GlobalSettings.Default.EnumMappingStrategy(EnumMappingStrategy.ByName);

        TypeAdapterConfig.GlobalSettings
            .When((src, dest, map) => src.GetInterface(nameof(IEntity)) is not null)
            .Map(nameof(IEntity.CreatedDateTime), (IEntity e) => e.CreatedDateTime.ToString(""dddd, dd MMMM yyyy HH:mm""))
            .Map(nameof(IEntity.ModifiedDateTime), (IEntity e) => e.ModifiedDateTime.ToString(""dddd, dd MMMM yyyy HH:mm""));

        ExtendRegister(config);

        config.GenerateMapper(""[name]Mapper"")");
        foreach (var entity in generationConfig.EntityConfigs)
        {
            sb.Append(@"
            .ForType<").Append(entity.BaseName).Append(@">()");
        }
        sb.Append(@";").Append(@"
    }
}

public static class AdaptAttributeBuilderExtensions
{
    public static AdaptAttributeBuilder ForTypeDefaultValues(this AdaptAttributeBuilder aab)
    {
        return aab");
        foreach (var entity in generationConfig.EntityConfigs)
        {
            sb.Append(@"
            .ForType<").Append(entity.BaseName).Append(@">(cfg =>
            {
                cfg.Map(poco => poco.Id, typeof(string));
                cfg.Map(poco => poco.CreatedDateTime, typeof(string));
                cfg.Map(poco => poco.ModifiedDateTime, typeof(string));
                // todo: enums?
                // foreach enum property in entity write cfg.map => enum, typeof(string)
            })");
        }
        sb.Append(@";
    }
}
");

        return sb.ToString();
    }

    internal static string EmitEntityModels(StringBuilder sb, string @namespace, EntityConfig entityConfig)
    {
        //todo: get extra namespaces from config entity
        sb.Clear();

        sb.Append(@"
using ApiAutoFast;
using System.ComponentModel.DataAnnotations;

namespace ").Append(@namespace).Append(@";

public class ").Append(entityConfig.BaseName).Append(@" : IEntity
{

    public ").Append(entityConfig.BaseName).Append(@"()
    {
        ");
        if (entityConfig.PropertyMetadatas?.Length > 0)
        {

            foreach (var propertyMetadata in entityConfig.PropertyMetadatas.Value)
            {
                if (propertyMetadata.Relational?.RelationalType is RelationalType.ToMany)
                {
                    sb
                        .Append(@"this.")
                        .Append(propertyMetadata.Relational.Value.ForeigEntityProperty)
                        .Append(@" = new HashSet<")
                        .Append(propertyMetadata.Relational.Value.ForeignEntityName)
                        .Append(@">();
");

                }
            }
        }
        sb.Append(@"
    }


    public Identifier Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }");

        if (entityConfig.PropertyMetadatas?.Length > 0)
        {
            foreach (var propertyMetadata in entityConfig.PropertyMetadatas.Value)
            {
                if (propertyMetadata.AttributeMetadatas?.Length > 0)
                {
                    foreach (var attributeMetadata in propertyMetadata.AttributeMetadatas.Value)
                    {
                        if (attributeMetadata.AttributeType is AttributeType.Appliable)
                        {
                            sb.Append(@"
    [").Append(attributeMetadata.Name).Append(@"]");
                        }
                    }
                }

                sb.Append(@"
    ").Append(propertyMetadata.Source);
            }
        }

        sb.Append(@"
}
");
        return sb.ToString();
    }

    internal static string EmitEndpoint(StringBuilder sb, string @namespace, EndpointConfig endpointConfig, string contextName)
    {
        sb.Clear();

        sb.Append(@"
using ApiAutoFast;
using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ").Append(@namespace).Append(@";
");
        sb.Append(@"
public partial class ")
            .Append(endpointConfig.Name)
            .Append(@" : Endpoint<")
            .Append(endpointConfig.Request)
            .Append(@", ")
            .Append(endpointConfig.Response)
            .Append(@", ")
            .Append(endpointConfig.MappingProfile)
            .Append(@">
{
    partial void OnExtendConfigure();
    private bool _extendConfigure = false;
    private readonly ").Append(contextName).Append(@" _dbContext;

    public ").Append(endpointConfig.Name).Append(@"(").Append(contextName).Append(@" dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        if (_extendConfigure is false)
        {
            Verbs(").Append(endpointConfig.RequestEndpointPair.HttpVerb).Append(@");
            Routes(""").Append(endpointConfig.Route).Append(@""");
            AllowAnonymous();
        }

        OnExtendConfigure();
    }
");
        sb.Append(@"
    public override async Task HandleAsync(").Append(endpointConfig.Request).Append(@" req, CancellationToken ct)
    {");
        sb = _endpointSourceGets(endpointConfig.RequestEndpointPair.EndpointTarget)(sb, endpointConfig).Append(@"
    }
}
");
        return sb.ToString();
    }

    internal static string EmitModelTarget(StringBuilder sb, string @namespace, EntityConfig entityConfig, string modelTarget)
    {
        sb.Clear();

        sb.Append(@"
using ApiAutoFast;

namespace ").Append(@namespace).Append(@";
");

        sb.Append(@"
public class ").Append(entityConfig.BaseName).Append(modelTarget).Append(@"
{");
        sb.Append(_getModelTargetSource(modelTarget));

        foreach (var propertySource in YieldModelTargetPropertySource(entityConfig, modelTarget))
        {
            sb.Append(@"
    ").Append(propertySource);
        }
        sb.Append(@"
}
");
        return sb.ToString();
    }

    internal static string EmitDbContext(StringBuilder sb, ContextGenerationConfig contextConfig, EntityGenerationConfig endpointsConfig)
    {
        sb.Clear();

        sb.Append(@"
using ApiAutoFast;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ").Append(endpointsConfig.Namespace).Append(@";

public partial class ").Append(contextConfig.Name).Append(@" : DbContext
{
    private static readonly Type[] _entityTypes;

    static ").Append(contextConfig.Name).Append(@"()
    {
        _entityTypes = AutoFastDbContextHelper.GetEntityTypes<").Append(contextConfig.Name).Append(@">();
    }

    public ").Append(contextConfig.Name).Append(@"(DbContextOptions<").Append(contextConfig.Name).Append(@"> options) : base(options) { }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AutoFastDbContextHelper.UpdateModifiedDateTime(ChangeTracker.Entries());

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AutoFastDbContextHelper.BuildEntities(modelBuilder, _entityTypes);
    }
");
        foreach (var entity in endpointsConfig.EntityConfigs)
        {
            sb.Append(@"
    public DbSet<").Append(entity.BaseName).Append("> ").Append(entity.BaseName).Append(@"s { get; init; } = default!;");
        }

        sb.Append(@"
}
");
        return sb.ToString();
    }

    internal static string EmitMappingProfile(StringBuilder sb, string @namespace, EntityConfig entityConfig)
    {
        sb.Clear();

        sb.Append(@"
using FastEndpoints;

namespace ").Append(@namespace).Append(@";

public partial class ")
    .Append(entityConfig.MappingProfile)
    .Append(@" : Mapper<")
    .Append(entityConfig.BaseName)
    .Append(nameof(AttributeModelTargetType.CreateCommand))
    .Append(@", ")
    .Append(entityConfig.Response)
    .Append(", ")
    .Append(entityConfig.BaseName)
    .Append(@">
{
    private readonly bool _onOverrideUpdateEntity = false;

    partial void")
    .Append(@" OnOverrideUpdateEntity(ref ")
    .Append(entityConfig.BaseName)
    .Append(@" originalEntity, ")
    .Append(entityConfig.BaseName)
    .Append(nameof(AttributeModelTargetType.ModifyCommand))
    .Append(@" e);

    public override ")
    .Append(entityConfig.Response)
    .Append(@" FromEntity(")
    .Append(entityConfig.BaseName)
    .Append(@" e)
    {
        return e.AdaptToResponse();
    }

    public ")
    .Append(entityConfig.BaseName)
    .Append(@" UpdateEntity(")
    .Append(entityConfig.BaseName)
    .Append(@" originalEntity, ")
    .Append(entityConfig.BaseName)
    .Append(nameof(AttributeModelTargetType.ModifyCommand))
    .Append(@" e)
    {
        if(_onOverrideUpdateEntity)
        {
            OnOverrideUpdateEntity(originalEntity, e);
            return originalEntity;
        }
");
        foreach (var propertyName in YieldModifyCommandProperties(entityConfig))
        {
            sb.Append(@"
        originalEntity.").Append(propertyName).Append(@" = e.").Append(propertyName).Append(';');
        }
        sb.Append(@"
        return originalEntity;
    }
}
");
        return sb.ToString();
    }

    private static IEnumerable<string> YieldModifyCommandProperties(EntityConfig entityConfig)
    {
        if (entityConfig.PropertyMetadatas?.Length > 0)
        {
            foreach (var propertyMetadata in entityConfig.PropertyMetadatas.Value)
            {
                if (propertyMetadata.AttributeMetadatas?.Length > 0
                    && propertyMetadata.AttributeMetadatas.Value.Any(x => x.Name is nameof(AttributeModelTargetType.ModifyCommand)))
                {
                    yield return propertyMetadata.Name;
                }
            }
        }
    }

    private static IEnumerable<string> YieldModelTargetPropertySource(EntityConfig entityConfig, string modelTarget)
    {
        if (entityConfig.PropertyMetadatas?.Length > 0)
        {
            foreach (var propertyMetadata in entityConfig.PropertyMetadatas.Value)
            {
                if (propertyMetadata.AttributeMetadatas?.Length > 0)
                {
                    var targetNames = propertyMetadata.AttributeMetadatas.Value
                        .Where(x => x.AttributeType is AttributeType.Target)
                        .Select(x => x.Name);

                    if (targetNames.Contains(modelTarget))
                    {
                        yield return propertyMetadata.Source;
                    }
                }
            }
        }
    }
}
