using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
using System.Text;

namespace ApiAutoFast.SourceGenerator.Emitters;

internal static class DefaultSourceEmitter
{
    private static readonly Func<RequestModelTarget, string> _getModelTargetSource = static (modelTarget) => modelTarget switch
    {
        RequestModelTarget.CreateCommand => string.Empty,
        RequestModelTarget.QueryRequest => @"
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }",
        _ => @"
    public string Id { get; set; }",
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
");
        var domainValueDefinitions = generationConfig.EntityConfigs
            .SelectMany(x => x.PropertyMetadatas)
            .Select(x => x.DomainValueDefiniton)
            .Distinct();

        foreach (var domainValueDefinition in domainValueDefinitions)
        {
            if (domainValueDefinition.ResponseType == domainValueDefinition.EntityType)
            {
                sb.Append(@"
        TypeAdapterConfig<")
                    .Append(domainValueDefinition.Name)
                    .Append(@", ")
                    .Append(domainValueDefinition.ResponseType)
                    .Append(@">.NewConfig().MapWith(x => x.EntityValue);");

                continue;
            }

            sb.Append(@"
        TypeAdapterConfig<")
                .Append(domainValueDefinition.Name)
                .Append(@", ")
                .Append(domainValueDefinition.ResponseType)
                .Append(@">.NewConfig().MapWith(x => x.ToString());");

        }

        sb.Append(@"

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
                cfg.Map(poco => poco.ModifiedDateTime, typeof(string));");
            foreach (var property in entity.PropertyMetadatas)
            {
                sb.Append(@"
                cfg.Map(poco => poco.").Append(property.Name).Append(@", typeof(").Append(property.DomainValueDefiniton.ResponseType).Append(@"));");
            }
            sb.Append(@"
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
using Microsoft.EntityFrameworkCore;

namespace ").Append(@namespace).Append(@";

public class ").Append(entityConfig.BaseName).Append(@" : IEntity
{
    public ").Append(entityConfig.BaseName).Append(@"()
    {");
        foreach (var propertyMetadata in entityConfig.PropertyMetadatas)
        {
            if (propertyMetadata.DomainValueDefiniton.PropertyRelation.RelationalType is RelationalType.ToMany)
            {
                sb.Append(@"
        this.").Append(propertyMetadata.DomainValueDefiniton.PropertyRelation.ForeigEntityProperty)
                .Append(@" = new HashSet<").Append(propertyMetadata.DomainValueDefiniton.PropertyRelation.ForeignEntityName)
                .Append(@">();");
            }
        }
        sb.Append(@"
    }

    public Identifier Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }");
        foreach (var propertyMetadata in entityConfig.PropertyMetadatas)
        {
            foreach (var attributeMetadata in propertyMetadata.AttributeMetadatas)
            {
                if (attributeMetadata.AttributeType is AttributeType.Default)
                {
                    sb.Append(@"
    [").Append(attributeMetadata.Name).Append(@"]");
                }
            }
            if (propertyMetadata.DomainValueDefiniton.PropertyRelation.RelationalType is RelationalType.ToOne)
            {
                sb.Append(@"
    public Identifier ").Append(propertyMetadata.DomainValueDefiniton.PropertyRelation.ForeignEntityName).Append(@"Id { get; set; }");
            }
            sb.Append(@"
    ").Append(propertyMetadata.EntitySource);
        }
        sb.Append(@"
}
");
        return sb.ToString();
    }

    internal static string EmitRequestModel(StringBuilder sb, string @namespace, EntityConfig entityConfig, RequestModelTarget modelTarget)
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

        foreach (var propertyMetadata in entityConfig.PropertyMetadatas)
        {
            if (propertyMetadata.RequestModelTarget.HasFlag(modelTarget))
            {
                if (modelTarget is RequestModelTarget.QueryRequest)
                {
                    sb.Append(@"
    ").Append(propertyMetadata.RequestSource);
                    continue;
                }
                sb.Append(@"
    ").Append(propertyMetadata.CommandSource);
            }
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

    partial void ExtendOnModelCreating(ModelBuilder modelBuilder);
    partial void ExtendSaveChanges();

    public ").Append(contextConfig.Name).Append(@"(DbContextOptions<").Append(contextConfig.Name).Append(@"> options) : base(options) { }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AutoFastDbContextHelper.UpdateModifiedDateTime(ChangeTracker.Entries());

        ExtendSaveChanges();

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        AutoFastDbContextHelper.BuildEntities(modelBuilder, _entityTypes);

        ExtendOnModelCreating(modelBuilder);
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
    .Append(entityConfig.CreateCommand)
    .Append(@", ")
    .Append(entityConfig.Response)
    .Append(", ")
    .Append(entityConfig.BaseName)
    .Append(@">
{
    private readonly bool _onOverrideUpdateEntity = false;

    partial void").Append(@" OnOverrideUpdateEntity(ref ").Append(entityConfig.BaseName).Append(@" originalEntity, ").Append(entityConfig.ModifyCommand).Append(@" e);

    public override ").Append(entityConfig.Response).Append(@" FromEntity(").Append(entityConfig.BaseName).Append(@" e)
    {
        return e.AdaptToResponse();
    }

    public ").Append(entityConfig.BaseName).Append(@" UpdateEntity(").Append(entityConfig.BaseName).Append(@" originalEntity, ").Append(entityConfig.ModifyCommand).Append(@" e)
    {
        if(_onOverrideUpdateEntity)
        {
            OnOverrideUpdateEntity(ref originalEntity, e);
            return originalEntity;
        }
");
        foreach (var propertyMetadata in entityConfig.PropertyMetadatas)
        {
            if (propertyMetadata.AttributeMetadatas.Any(x => x.Name is nameof(RequestModelTarget.ModifyCommand)))
            {
                sb.Append(@"
        originalEntity.").Append(propertyMetadata.Name).Append(@" = e.").Append(propertyMetadata.Name).Append(';');
            }
        }
        sb.Append(@"
        return originalEntity;
    }

    public ").Append(entityConfig.BaseName).Append(@" ToDomainEntity(").Append(entityConfig.CreateCommand).Append(@" command, Action<string, string> addValidationError)
    {
        return new ").Append(entityConfig.BaseName).Append(@"
        {
");
        foreach (var property in entityConfig.PropertyMetadatas)
        {
            //   var valueTypeDefault = property.DomainValueDefiniton.RequestIsValueType ? @" ?? 0" : string.Empty;

            sb.Append(@"            ")
                .Append(property.Name)
                .Append(@" = ")
                .Append(property.DomainValueDefiniton.Name)
                .Append(@".ConvertFromRequest(command.")
                .Append(property.Name)
                .Append(@", addValidationError),
");
        }
        sb.Append(@"        };
    }
}
");
        return sb.ToString();
    }
}
