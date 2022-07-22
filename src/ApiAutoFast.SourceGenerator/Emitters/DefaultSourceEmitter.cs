using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
using ApiAutoFast.SourceGenerator.Descriptive;
using System.Text;

namespace ApiAutoFast.SourceGenerator.Emitters;

internal static class DefaultSourceEmitter
{
    private static readonly Func<RequestModelTarget, string> _getModelTargetSource = static (modelTarget) => modelTarget switch
    {
        RequestModelTarget.CreateCommand => "",
        RequestModelTarget.QueryRequest => @"
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }",
        _ => @"
    public string Id { get; set; } = default!;",
    };

    internal static string EmitEntityModel(StringBuilder sb, string @namespace, EntityConfig entityConfig)
    {
        sb.Clear();

        sb.Append(@"
#nullable enable

using ApiAutoFast;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ").Append(@namespace).Append(@";

[Index(nameof(CreatedDateTime), nameof(Id))]
public class ").Append(entityConfig.BaseName).Append(@" : IEntity<").Append(entityConfig.EndpointsAttributeArguments.IdType).Append(@">
{
    public ").Append(entityConfig.BaseName).Append(@"()
    {");

        var entityProperties = entityConfig.PropertyConfig.Properties
            .Where(x => x.Target is PropertyTarget.Entity)
            .ToArray();

        foreach (var propertyOutput in entityProperties)
        {
            if (propertyOutput.Relation.Type is RelationalType.ToMany)
            {
                sb.Append(@"
        this.").Append(propertyOutput.Relation.ForeigEntityProperty)
                .Append(@" = new HashSet<")
                .Append(propertyOutput.Relation.ForeignEntityName)
                .Append(@">();");
            }
        }
        sb.Append(@"
    }

    public ").Append(entityConfig.EndpointsAttributeArguments.IdType).Append(@" Id { get; set; } = default!;
    public DateTime CreatedDateTime { get; set; } = default!;
    public DateTime ModifiedDateTime { get; set; } = default!;");
        foreach (var propertyOutput in entityProperties)
        {
            if (propertyOutput.Relation.Type is RelationalType.ToOne)
            {
                sb.Append(@"
    [Required]
    ").Append(propertyOutput.Source).Append(@" = default!;");
                continue;
            }
            sb.Append(@"
    ").Append(propertyOutput.Source);
        }
        sb.Append(@"
}
");
        return sb.ToString();
    }

    internal static string EmitRequestModel(StringBuilder sb, string @namespace, EntityConfig entityConfig, RequestModelTarget modelTarget, PropertyTarget propertyTarget)
    {
        sb.Clear();

        sb.Append(@"
 #nullable enable

using ApiAutoFast;

namespace ").Append(@namespace).Append(@";
");
        sb.Append(@"
public partial class ").Append(entityConfig.BaseName).Append(modelTarget).Append(@"
{");
        sb.Append(_getModelTargetSource(modelTarget));

        if (propertyTarget is not PropertyTarget.None)
        {
            foreach (var property in entityConfig.PropertyConfig.Properties.Where(x => x.Target.HasFlag(propertyTarget)))
            {
                if (property.Relation.Type is RelationalType.ToMany && propertyTarget is PropertyTarget.QueryRequest) continue;

                sb.Append(@"
    ").Append(property.Source);
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
    public DbSet<").Append(entity.BaseName).Append("> ").Append(entity.BaseName).Append(@"s => Set<").Append(entity.BaseName).Append(@">();");
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
    public override ").Append(entityConfig.Response).Append(@" FromEntity(").Append(entityConfig.BaseName).Append(@" e)
    {
        return e.MapToResponse();
    }

    public ").Append(entityConfig.BaseName).Append(@" UpdateDomainEntity(").Append(entityConfig.BaseName).Append(@" entity, ").Append(entityConfig.ModifyCommand).Append(@" command, Action<string, string> addValidationError)
    {");
        var domainValues = entityConfig.PropertyConfig.DomainValues
            .Where(x => x.DomainValueDefinition.PropertyRelation.Type is not RelationalType.ToMany)
            .ToArray();

        foreach (var domainValue in domainValues)
        {
            foreach (var property in domainValue.DefinedProperties)
            {
                if (property.PropertyKind is PropertyKind.Identifier && domainValue.DomainValueDefinition.PropertyRelation.Type is RelationalType.ToOne)
                {
                    sb.Append(@"
        entity.").Append(property.Name).Append(@" = ").Append(TypeText.IdentifierUtility).Append(@".ConvertFromRequest<").Append(property.Type).Append(@">(command.").Append(property.Name).Append(@", addValidationError);");
                    continue;
                }

                if (property.PropertyKind is PropertyKind.Domain && domainValue.DomainValueDefinition.PropertyRelation.Type is RelationalType.None)
                {
                    sb.Append(@"
        entity.").Append(property.Name).Append(@" = ").Append(property.Type).Append(".UpdateFromRequest(entity.").Append(property.Name).Append(", command.").Append(property.Name).Append(@", addValidationError);");
                }
            }
        }

        sb.Append(@"
        return entity;
    }

    public ").Append(entityConfig.BaseName).Append(@" ToDomainEntity(").Append(entityConfig.CreateCommand).Append(@" command, Action<string, string> addValidationError)
    {
        return new ").Append(entityConfig.BaseName).Append(@"
        {");
        foreach (var domainValue in domainValues)
        {
            foreach (var property in domainValue.DefinedProperties)
            {
                if (property.PropertyKind is PropertyKind.Identifier && domainValue.DomainValueDefinition.PropertyRelation.Type is RelationalType.ToOne)
                {
                    sb.Append(@"
            ").Append(property.Name).Append(@" = ").Append(TypeText.IdentifierUtility).Append(@".ConvertFromRequest<").Append(property.Type).Append(@">(command.").Append(property.Name).Append(@", addValidationError),");
                    continue;
                }

                if (property.PropertyKind is PropertyKind.Domain && domainValue.DomainValueDefinition.PropertyRelation.Type is RelationalType.None)
                {
                    sb.Append(@"
            ").Append(property.Name).Append(@" = ").Append(property.Type).Append(@".ConvertFromRequest(command.").Append(property.Name).Append(@", addValidationError),");
                }
            }
        }
        sb.Append(@"
        };
    }
}
");
        return sb.ToString();
    }
}
