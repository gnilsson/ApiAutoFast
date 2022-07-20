using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
using System.Collections.Immutable;
using System.Text;

namespace ApiAutoFast.SourceGenerator.Emitters;

internal static class MapperSourceEmitter
{
    internal static IEnumerable<(string Name, string Source)> EmitMappers(StringBuilder sb, string @namespace, ImmutableArray<EntityConfig> entityConfigs)
    {
        foreach (var entityConfig in entityConfigs)
        {
            var funcCount = entityConfig.PropertyConfig.DomainValues.Count(x => x.DomainValueDefinition.PropertyRelation.Type is not RelationalType.None);

            var param = Enumerable.Range(0, funcCount + 1).Select(x => $"p{x}").ToArray();
            var func = Enumerable.Range(0, funcCount).Select(x => $"MapSimple{x}").ToArray();

            var iterator = 0;
            var funcNamesBuilder = ImmutableArray.CreateBuilder<(string Response, string Entity, RelationalType Type)>();

            sb.Clear();

            sb.Append(@"
#nullable enable

using ApiAutoFast;

namespace ").Append(@namespace).Append(@";

public static partial class ").Append(entityConfig.BaseName).Append(@"Mapper2
{
    public static ").Append(entityConfig.Response).Append(@"2 MapToResponse(this ").Append(entityConfig.BaseName).Append(@" ").Append(param[0]).Append(@")
    {
        if(").Append(param[0]).Append(@" is null) return null!;

        return new ").Append(entityConfig.Response).Append(@"2
        {
            Id = ").Append(param[0]).Append(@".Id.ToString(),
            CreatedDateTime = ((ITimestamp)").Append(param[0]).Append(@").CreatedDateTime.ToString(""dddd, dd MMMM yyyy HH: mm""),
            ModifiedDateTime = ((ITimestamp)").Append(param[0]).Append(@").ModifiedDateTime.ToString(""dddd, dd MMMM yyyy HH:mm""),");

            foreach (var domainValue in entityConfig.PropertyConfig.DomainValues)
            {
                foreach (var property in domainValue.DefinedProperties)
                {
                    if (domainValue.DomainValueDefinition.PropertyRelation.Type is not RelationalType.None)
                    {
                        (var eh, var ho, var ah) = ($"{domainValue.DomainValueDefinition.EntityType}ResponseSimplified", domainValue.DomainValueDefinition.EntityType, domainValue.DomainValueDefinition.PropertyRelation.Type);
                        funcNamesBuilder.Add((eh, ho, ah));

                        sb.Append(@"
            ").Append(domainValue.DomainValueDefinition.EntityType).Append(@" = ").Append(func[iterator++]).Append("(").Append(param[0]).Append(".").Append(domainValue.DomainValueDefinition.EntityType).Append("),");
                        continue;
                    }

                    sb.Append(@"
            ").Append(property.Name).Append(@" = ").Append(param[0]).Append(".").Append(property.Name).Append(@"?.ToResponse(),");
                }
            }

            sb.Append(@"
        };
    }
");
            var funcNames = funcNamesBuilder.ToImmutable();

            for (int i = 0; i < iterator; i++)
            {
                //temp
                if (funcNames[i].Type is RelationalType.ToMany) continue;

                sb.Append(@"
    private static ").Append(funcNames[i].Response).Append(" ").Append(func[i]).Append('(').Append(funcNames[i].Entity).Append(' ').Append(param[i + 1]).Append(@")
    {
        if(").Append(param[i + 1]).Append(@" is null) return null!;

        return new ").Append(funcNames[i].Response).Append(@"
        {
            Id = ").Append(param[i + 1]).Append(@".Id.ToString(),
            CreatedDateTime = ((ITimestamp)").Append(param[i + 1]).Append(@").CreatedDateTime.ToString(""dddd, dd MMMM yyyy HH: mm""),
            ModifiedDateTime = ((ITimestamp)").Append(param[i + 1]).Append(@").ModifiedDateTime.ToString(""dddd, dd MMMM yyyy HH:mm""),");

                var foreignConfig = entityConfigs.First(x => x.BaseName == funcNames[i].Entity);

                foreach (var domainValue in foreignConfig.PropertyConfig.DomainValues)
                {
                    foreach (var property in domainValue.DefinedProperties)
                    {
                        //            if (property.PropertyKind is PropertyKind.Identifier)
                        //            {
                        //                sb.Append(@"
                        //.").Append(property.Name).Append(@" = ").Append(param[0]).Append(".").Append(property.Name).Append(@".ToString();");
                        //                continue;
                        //            }

                        //                if (domainValue.DomainValueDefinition.PropertyRelation.Type is not RelationalType.None)
                        //                {
                        //                    sb.Append(@"
                        //result.").Append(property.Name).Append(@" = ").Append(func[i]).Append("(").Append(param[i + 1]).Append(".").Append(property.Name).Append(");");
                        //                    continue;
                        //                }

                        //                if (domainValue.DomainValueDefinition.PropertyRelation.Type is RelationalType.ToMany)
                        //                {
                        //                    sb.Append(@"
                        //result.").Append(property.Name).Append(@" = Array.Empty<").Append(domainValue.DomainValueDefinition.ResponseType).Append(">();");
                        //                    continue;
                        //                }

                        sb.Append(@"
            ").Append(property.Name).Append(@" = ").Append(param[i + 1]).Append(".").Append(property.Name).Append(@"?.ToResponse(),");
                    }
                }
                sb.Append(@"
        };
    }");
            }
            sb.Append(@"
}
");
            yield return (entityConfig.BaseName, sb.ToString());
        }
    }

    internal static string EmitResponseModel(StringBuilder sb, string @namespace, EntityConfig entityConfig)
    {
        sb.Clear();

        sb.Append(@"
#nullable enable

using ApiAutoFast;

namespace ").Append(@namespace).Append(@";

public partial class ").Append(entityConfig.Response).Append(@"2
{
    public string? Id { get; set; }
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }");
        foreach (var property in entityConfig.PropertyConfig.Properties.Where(x => x.Target is PropertyTarget.Response))
        {
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
    public string? Id { get; set; }
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }");
        foreach (var property in entityConfig.PropertyConfig.Properties.Where(x => x.Target is PropertyTarget.Response && x.Relation.Type is not RelationalType.ToMany))
        {
            sb.Append(@"
    ").Append(property.Source);
        }

        sb.Append(@"
}
");
        return sb.ToString();
    }
}
