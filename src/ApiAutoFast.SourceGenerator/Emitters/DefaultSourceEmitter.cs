﻿using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
using ApiAutoFast.SourceGenerator.Descriptive;
using System.Collections.Immutable;
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

        TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
        TypeAdapterConfig.GlobalSettings.Default.MaxDepth(2);
        TypeAdapterConfig.GlobalSettings.Default.ShallowCopyForSameType(true);
        TypeAdapterConfig.GlobalSettings.Default.EnumMappingStrategy(EnumMappingStrategy.ByName);
        TypeAdapterConfig.GlobalSettings.Default.AddDestinationTransform(DestinationTransform.EmptyCollectionIfNull);

        TypeAdapterConfig.GlobalSettings
            .When((src, dest, map) => src.GetInterface(nameof(ITimestamp)) is not null)
            .Map(nameof(ITimestamp.CreatedDateTime), (ITimestamp e) => e.CreatedDateTime.ToString(""dddd, dd MMMM yyyy HH:mm""))
            .Map(nameof(ITimestamp.ModifiedDateTime), (ITimestamp e) => e.ModifiedDateTime.ToString(""dddd, dd MMMM yyyy HH:mm""));");

        foreach (var definedDomainValue in generationConfig.EntityConfigs
            .SelectMany(x => x.PropertyConfig.DomainValues)
            .Distinct())
        {
            if (definedDomainValue.DomainValueDefinition.PropertyRelation.Type is not RelationalType.None) continue;

            if (definedDomainValue.DomainValueDefinition.ResponseType == definedDomainValue.DomainValueDefinition.EntityType)
            {
                sb.Append(@"
            TypeAdapterConfig<")
                    .Append(definedDomainValue.DomainValueDefinition.TypeName)
                    .Append(@", ")
                    .Append(definedDomainValue.DomainValueDefinition.ResponseType)
                    .Append(@">.NewConfig().MapWith(x => x.EntityValue);");

                continue;
            }

            sb.Append(@"
        TypeAdapterConfig<")
                .Append(definedDomainValue.DomainValueDefinition.TypeName)
                .Append(@", ")
                .Append(definedDomainValue.DomainValueDefinition.ResponseType)
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
            foreach (var definedDomainValue in entity.PropertyConfig.DomainValues)
            {
                foreach (var property in definedDomainValue.DefinedProperties)
                {
                    sb.Append(@"
                    cfg.Map(poco => poco.").Append(property.Name).Append(@", typeof(").Append(definedDomainValue.DomainValueDefinition.ResponseType).Append(@"));");
                }
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
        var properties = entityConfig.PropertyConfig.DomainValues
            .Where(x => x.DomainValueDefinition.PropertyRelation.Type is not RelationalType.ToMany)
            .SelectMany(x => x.DefinedProperties)
            .ToArray();

        foreach (var property in properties)
        {
            if (property.Type is not TypeText.Identifier and not TypeText.SequentialIdentifier)
            {
                sb.Append(@"
        entity.").Append(property.Name).Append(@" = ").Append(property.Type).Append(".UpdateFromRequest(entity.").Append(property.Name).Append(", command.").Append(property.Name).Append(@", addValidationError);");
            }
        }
        sb.Append(@"
        return entity;
    }

    public ").Append(entityConfig.BaseName).Append(@" ToDomainEntity(").Append(entityConfig.CreateCommand).Append(@" command, Action<string, string> addValidationError)
    {
        return new ").Append(entityConfig.BaseName).Append(@"
        {");
        foreach (var property in properties)
        {
            if (property.Type is TypeText.Identifier or TypeText.SequentialIdentifier)
            {
                sb.Append(@"
            ").Append(property.Name).Append(@" = ").Append(TypeText.IdentifierUtility).Append(@".ConvertFromRequest<").Append(property.Type).Append(@">(command.").Append(property.Name).Append(@", addValidationError),");
                continue;
            }

            sb.Append(@"
            ").Append(property.Name).Append(@" = ").Append(property.Type).Append(@".ConvertFromRequest(command.").Append(property.Name).Append(@", addValidationError),");
        }
        sb.Append(@"
        };
    }
}
");
        return sb.ToString();
    }
}
