using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
using System.Collections.Immutable;
using System.Text;

namespace ApiAutoFast.SourceGenerator.Emitters;

internal static class MapperSourceEmitter
{
    private class SimpleMap
    {
        public string Response { get; set; } = default!;
        public string Entity { get; set; } = default!;
        public RelationalType Type { get; set; } = default!;
        public string FuncName { get; set; } = default!;
        public string ParamName { get; set; } = default!;
        public string ParamType { get; set; } = default!;
        public string EntityType { get; set; } = default!;
    }

    //private class EnumerableMap
    //{
    //    public string Response { get; set; } = default!;
    //    public string Entity { get; set; } = default!;
    //    public ImmutableArray<SimpleMap>? SimpleMaps { get; set; } = default!;
    //}

    internal static IEnumerable<(string Name, string Source)> EmitMappers(StringBuilder sb, string @namespace, ImmutableArray<EntityConfig> entityConfigs)
    {
        foreach (var entityConfig in entityConfigs)
        {
            //var funcCount = entityConfig.PropertyConfig.DomainValues.Count(x => x.DomainValueDefinition.PropertyRelation.Type is RelationalType.ToOne);
            //var enumerableFuncCount = entityConfig.PropertyConfig.DomainValues.Count(x => x.DomainValueDefinition.PropertyRelation.Type is RelationalType.ToOne);

          //  var param = new[] { "p0" };

            //var param = Enumerable.Range(0, funcCount + 1).Select(x => $"p{x}").ToArray();
            //var funcNames = Enumerable.Range(0, funcCount).Select(x => $"MapSimple{x}").ToArray();
            //var enumerableFunc = Enumerable.Range(0, enumerableFuncCount).Select(x => $"MapEnumerable{x}").ToArray();

            //var enumerableIterator = 0;
            var iterator = 0;

            //var funcNamesBuilder = ImmutableArray.CreateBuilder<(string Response, string Entity, RelationalType Type)>();
            //var enumerableFuncsBuilder = ImmutableArray.CreateBuilder<(string Response, string Entity)>();

            var funcBuilder = ImmutableArray.CreateBuilder<SimpleMap>();
            //     var enumerableFuncBuilder = ImmutableArray.CreateBuilder<EnumerableMap>();

            sb.Clear();

            sb.Append(@"
#nullable enable

using ApiAutoFast;
using System.Linq;

namespace ").Append(@namespace).Append(@";

public static partial class ").Append(entityConfig.BaseName).Append(@"Mapper2
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

                    if (domainValue.DomainValueDefinition.PropertyRelation.Type is not RelationalType.None)
                    {
                        //    $"{domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName}ResponseSimplified",
                        //    domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName,
                        //    domainValue.DomainValueDefinition.PropertyRelation.Type);

                        //funcNamesBuilder.Add((eh, ho, ah));
                        // add map enumerable
                        string funcName;
                        if (domainValue.DomainValueDefinition.PropertyRelation.Type is RelationalType.ToOne)
                        {
                            funcName = $"MapSimple{iterator}";
                            funcBuilder.Add(new SimpleMap
                            {
                                Entity = domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName,
                                Response = $"{domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName}ResponseSimplified",
                                FuncName = funcName,
                                ParamName = $"p{iterator + 1}",
                                Type = RelationalType.ToOne,
             //                   ParamType = domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName,
                            });
                        }
                        else
                        {
                        //    var type = domainValue.DomainValueDefinition.EntityType.Replace("ICollection", "IEnumerable");
                            funcName = $"MapEnumerable{iterator}";
                            funcBuilder.Add(new SimpleMap
                            {
                                Entity = domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName,
                                Response = $"{domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName}ResponseSimplified",
                                //       Response = type.Replace(domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName, $"{domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName}ResponseSimplified"),
                                FuncName = funcName,
                                ParamName = $"ep{iterator + 1}",
                                Type = RelationalType.ToMany,
                                EntityType = domainValue.DomainValueDefinition.EntityType,
                            });
                        }


                        sb.Append(@"
            ").Append(domainValue.DomainValueDefinition.PropertyRelation.ForeigEntityProperty).Append(@" = ").Append(funcName).Append("(p.").Append(domainValue.DomainValueDefinition.PropertyRelation.ForeigEntityProperty).Append("),");

                        iterator++;
                        continue;
                    }

                    sb.Append(@"
            ").Append(property.Name).Append(@" = p.").Append(property.Name).Append(@"?.ToResponse(),");
                }
            }

            sb.Append(@"
        };
    }
");
            var funcs = funcBuilder.ToImmutable();

            for (int i = 0; i < iterator; i++)
            {

                //temp
                //       if (funcNames[i].Type is RelationalType.ToMany) continue;

                //  var response = funcNames[i].Type is RelationalType.ToMany ?

                var func = funcs[i];
                var foreignDomainValues = entityConfigs.FirstOrDefault(x => x.BaseName == func.Entity).PropertyConfig.DomainValues;
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
            yield return (entityConfig.BaseName, sb.ToString());
        }
    }

    private static StringBuilder BuildToManyFunc(StringBuilder sb, ImmutableArray<DefinedDomainValue> foreignDomainValues, SimpleMap func)
    {
        var type = func.EntityType.Replace("ICollection", "IEnumerable");
        var response = type.Replace(func.Entity, func.Response);

        sb.Append(@"
    private static ").Append(response).Append(" ").Append(func.FuncName).Append('(').Append(type).Append(' ').Append(func.ParamName).Append(@")
    {
        if (").Append(func.ParamName).Append(@" is null) yield break;

        foreach (var p in ").Append(func.ParamName).Append(@")
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

    private static StringBuilder BuildToOneFunc(StringBuilder sb, ImmutableArray<DefinedDomainValue> foreignDomainValues, SimpleMap func)
    {
        sb.Append(@"
    private static ").Append(func.Response).Append(" ").Append(func.FuncName).Append('(').Append(func.Entity).Append(' ').Append(func.ParamName).Append(@")
    {
        if (").Append(func.ParamName).Append(@" is null) return null!;

        return new ").Append(func.Response).Append(@"
        {
            Id = ").Append(func.ParamName).Append(@".Id.ToString(),
            CreatedDateTime = ").Append(func.ParamName).Append(@".CreatedDateTime.ToString(""dddd, dd MMMM yyyy HH: mm""),
            ModifiedDateTime = ").Append(func.ParamName).Append(@".ModifiedDateTime.ToString(""dddd, dd MMMM yyyy HH:mm""),");

        foreach (var domainValue in foreignDomainValues)
        {
            foreach (var property in domainValue.DefinedProperties)
            {
                if (domainValue.DomainValueDefinition.PropertyRelation.Type is RelationalType.ToOne)
                {
                    if (property.PropertyKind is PropertyKind.Identifier)
                    {
                        sb.Append(@"
            ").Append(domainValue.DomainValueDefinition.PropertyRelation.ForeignEntityName).Append(@" = ").Append(func.ParamName).Append(".").Append(property.Name).Append(@".ToString(),");
                    }
                    continue;
                }

                sb.Append(@"
            ").Append(property.Name).Append(@" = ").Append(func.ParamName).Append(".").Append(property.Name);

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
