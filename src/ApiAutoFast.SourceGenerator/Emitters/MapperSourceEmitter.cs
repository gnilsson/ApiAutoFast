using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
using System.Collections.Immutable;
using System.Text;

namespace ApiAutoFast.SourceGenerator.Emitters;

internal static class MapperSourceEmitter
{
    private class MapFuncMetadata
    {
        public string Response { get; set; } = default!;
        public string Entity { get; set; } = default!;
        public RelationalType Type { get; set; } = default!;
        public string FuncName { get; set; } = default!;
        public string EntityType { get; set; } = default!;
    }

    internal static string EmitMapper(StringBuilder sb, string @namespace, EntityConfig entityConfig, ImmutableArray<EntityConfig> foreignConfigs)
    {
        sb.Clear();

        var iterator = 0;
        var funcBuilder = ImmutableArray.CreateBuilder<MapFuncMetadata>();

        sb.Append(@"
#nullable enable

using ApiAutoFast;
using System.Linq;

namespace ").Append(@namespace).Append(@";

public static partial class ").Append(entityConfig.BaseName).Append(@"Mapper
{
    public static ").Append(entityConfig.Response).Append(@" MapToResponse(this ").Append(entityConfig.BaseName).Append(@" p)
    {
        if (p is null) return null!;

        return new ").Append(entityConfig.Response).Append(@"
        {
            Id = p.Id.ToString(),
            CreatedDateTime = p.CreatedDateTime.ToString(""dddd, dd MMMM yyyy HH: mm""),
            ModifiedDateTime = p.ModifiedDateTime.ToString(""dddd, dd MMMM yyyy HH:mm""),");

        foreach (var domainValue in entityConfig.PropertyConfig.DomainValues)
        {
            foreach (var property in domainValue.DefinedProperties)
            {
                if (property.PropertyKind is PropertyKind.Identifier) continue;

                if (domainValue.DomainValueDefinition.PropertyRelation.Type is RelationalType.None)
                {
                    sb.Append(@"
            ").Append(property.Name).Append(@" = p.").Append(property.Name).Append(@"?.ToResponse(),");
                    continue;
                }

                var mapFunc = domainValue.DomainValueDefinition.PropertyRelation.Type switch
                {
                    RelationalType.ToOne => new MapFuncMetadata
                    {
                        Entity = domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName,
                        Response = $"{domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName}ResponseSimplified",
                        FuncName = $"MapSimple{iterator}",
                        Type = RelationalType.ToOne,
                    },
                    RelationalType.ToMany => new MapFuncMetadata
                    {
                        Entity = domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName,
                        Response = $"{domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName}ResponseSimplified",
                        FuncName = $"MapEnumerable{iterator}",
                        Type = RelationalType.ToMany,
                        EntityType = domainValue.DomainValueDefinition.EntityType,
                    },
                    _ => null
                };

                funcBuilder.Add(mapFunc!);

                sb.Append(@"
            ").Append(domainValue.DomainValueDefinition.PropertyRelation.ForeigEntityProperty).Append(@" = ").Append(mapFunc!.FuncName).Append("(p.").Append(domainValue.DomainValueDefinition.PropertyRelation.ForeigEntityProperty).Append("),");

                iterator++;
            }
        }

        sb.Append(@"
        };
    }
");
        var funcs = funcBuilder.ToImmutable();

        for (int i = 0; i < iterator; i++)
        {
            var func = funcs[i];
            var foreignDomainValues = foreignConfigs.FirstOrDefault(x => x.BaseName == func.Entity).PropertyConfig.DomainValues;

            if (func.Type is RelationalType.ToMany)
            {
                sb = BuildToManyFunc(sb, foreignDomainValues, func);
                continue;
            }

            sb = BuildToOneFunc(sb, foreignDomainValues, func);
        }
        sb.Append(@"
}
");

        return sb.ToString();
    }

    private static StringBuilder BuildToManyFunc(StringBuilder sb, ImmutableArray<DefinedDomainValue> foreignDomainValues, MapFuncMetadata func)
    {
        var type = func.EntityType.Replace("ICollection", "IEnumerable");
        var response = type.Replace(func.Entity, func.Response);

        sb.Append(@"
    private static ").Append(response).Append(" ").Append(func.FuncName).Append('(').Append(type).Append(@" ep)
    {
        if (ep is null) yield break;

        foreach (var p in ep)
        {
            yield return new ").Append(func.Response).Append(@"
            {
                Id = p.Id.ToString(),
                CreatedDateTime = p.CreatedDateTime.ToString(""dddd, dd MMMM yyyy HH: mm""),
                ModifiedDateTime = p.ModifiedDateTime.ToString(""dddd, dd MMMM yyyy HH:mm""),");

        foreach (var domainValue in foreignDomainValues)
        {
            foreach (var property in domainValue.DefinedProperties)
            {
                if (domainValue.DomainValueDefinition.PropertyRelation.Type is RelationalType.ToOne)
                {
                    if (property.PropertyKind is PropertyKind.Identifier)
                    {
                        sb.Append(@"
                ").Append(domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName).Append(@" = p.").Append(property.Name).Append(@".ToString(),");
                    }
                    continue;
                }

                sb.Append(@"
                ").Append(property.Name).Append(@" = p.").Append(property.Name);

                if (domainValue.DomainValueDefinition.PropertyRelation.Type is RelationalType.ToMany)
                {
                    sb.Append(@"?.Select(x => x.Id.ToString()),");
                    continue;
                }

                sb.Append(@"?.ToResponse(),");
            }
        }
        sb.Append(@"
            };
        }
    }
");
        return sb;
    }

    private static StringBuilder BuildToOneFunc(StringBuilder sb, ImmutableArray<DefinedDomainValue> foreignDomainValues, MapFuncMetadata func)
    {
        sb.Append(@"
    private static ").Append(func.Response).Append(" ").Append(func.FuncName).Append('(').Append(func.Entity).Append(@" p)
    {
        if (p is null) return null!;

        return new ").Append(func.Response).Append(@"
        {
            Id = p.Id.ToString(),
            CreatedDateTime = p.CreatedDateTime.ToString(""dddd, dd MMMM yyyy HH: mm""),
            ModifiedDateTime = p.ModifiedDateTime.ToString(""dddd, dd MMMM yyyy HH:mm""),");

        foreach (var domainValue in foreignDomainValues)
        {
            foreach (var property in domainValue.DefinedProperties)
            {
                if (domainValue.DomainValueDefinition.PropertyRelation.Type is RelationalType.ToOne)
                {
                    if (property.PropertyKind is PropertyKind.Identifier)
                    {
                        sb.Append(@"
            ").Append(domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName).Append(@" = p.").Append(property.Name).Append(@".ToString(),");
                    }
                    continue;
                }

                sb.Append(@"
            ").Append(property.Name).Append(@" = p.").Append(property.Name);

                if (domainValue.DomainValueDefinition.PropertyRelation.Type is RelationalType.ToMany)
                {
                    sb.Append(@"?.Select(x => x.Id.ToString()),");
                    continue;
                }

                sb.Append(@"?.ToResponse(),");
            }
        }
        sb.Append(@"
        };
    }
");
        return sb;
    }


    internal static string EmitResponseModel(StringBuilder sb, string @namespace, EntityConfig entityConfig)
    {
        sb.Clear();

        sb.Append(@"
#nullable enable

using ApiAutoFast;

namespace ").Append(@namespace).Append(@";

public partial class ").Append(entityConfig.Response).Append(@"
{
    public string Id { get; init; } = default!;
    public string CreatedDateTime { get; init; } = default!;
    public string ModifiedDateTime { get; init; } = default!;");
        foreach (var property in entityConfig.PropertyConfig.Properties.Where(x => x.Target is PropertyTarget.Response))
        {
            if (property.Relation.Type is not RelationalType.None && property.PropertyKind is PropertyKind.Identifier) continue;

            sb.Append(@"
    ").Append(property.Source);
        }

        sb.Append(@"
}
");
        return sb.ToString();
    }

    internal static string EmitSimplifiedResponseModel(StringBuilder sb, string @namespace, EntityConfig entityConfig)
    {
        sb.Clear();

        sb.Append(@"
#nullable enable

using ApiAutoFast;

namespace ").Append(@namespace).Append(@";

public partial class ").Append(entityConfig.Response).Append(@"Simplified
{
    public string Id { get; init; } = default!;
    public string CreatedDateTime { get; init; } = default!;
    public string ModifiedDateTime { get; init; } = default!;");
        foreach (var property in entityConfig.PropertyConfig.Properties.Where(x => x.Target is PropertyTarget.Response))
        {
            if (property.Relation.Type is not RelationalType.None && property.PropertyKind is not PropertyKind.Identifier) continue;

            sb.Append(@"
    ").Append(property.Source);
        }

        sb.Append(@"
}
");
        return sb.ToString();
    }
}
