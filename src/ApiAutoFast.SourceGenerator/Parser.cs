using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator;

internal static class Parser
{
    private const string AutoFastEndpointsAttribute = "ApiAutoFast.AutoFastEndpointsAttribute";
    private const string AutoFastContextAttribute = "ApiAutoFast.AutoFastContextAttribute";
    private const string DomainValueAttribute = "ApiAutoFast.DomainValueAttribute";

    internal static SemanticTargetInformation? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;

                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName is AutoFastEndpointsAttribute or AutoFastContextAttribute or DomainValueAttribute)
                {
                    return new(classDeclarationSyntax, fullName);
                }
            }
        }

        return null;
    }

    internal static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0;

    internal readonly struct DomainValueDefinition
    {
        public DomainValueDefinition(ITypeSymbol requestType, ITypeSymbol entityType, string domainValueName)
        {
            RequestType = requestType;
            EntityType = entityType;
            DomainValueName = domainValueName;
        }

        public ITypeSymbol RequestType { get; }
        public ITypeSymbol EntityType { get; }
        public string DomainValueName { get; }
    }

    //private static IEnumerable<DomainValueDefinitionMetadata> YieldDomainValueDefinitionMetadata(Compilation compilation, ImmutableArray<SemanticTargetInformation> semanticTargetInformations)
    //{
    //    foreach (var semanticTargetInformation in semanticTargetInformations)
    //    {
    //        if (semanticTargetInformation.Target is DomainValueAttribute)
    //        {
    //            var semanticModel = compilation.GetSemanticModel(semanticTargetInformation.ClassDeclarationSyntax.SyntaxTree);

    //            if (semanticModel.GetDeclaredSymbol(semanticTargetInformation.ClassDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol) continue;

    //            if (namedTypeSymbol.BaseType?.TypeArguments.Length > 0)
    //            {
    //                var domainValueTypes = namedTypeSymbol.BaseType.TypeArguments;
    //                var requestTypeSymbol = domainValueTypes[0];
    //                var entityTypeSymbol = domainValueTypes.Length == 1 ? requestTypeSymbol : domainValueTypes[1];

    //                yield return new DomainValueDefinitionMetadata(requestTypeSymbol, entityTypeSymbol, namedTypeSymbol.Name);
    //            }


    //            //var symbol = entityConfigSetup.SemanticModel.GetDeclaredSymbol(domainValueDefinition);

    //            //if (symbol?.BaseType?.TypeArguments.Length > 0)
    //            //{
    //            //    // get name of class and save types

    //            //}
    //        }
    //    }
    //}

    internal static GenerationConfig? GetGenerationConfig(Compilation compilation, ImmutableArray<SemanticTargetInformation> semanticTargetInformations, CancellationToken ct)
    {
        //var endpointAttribute = compilation.GetTypeByMetadataName(AutoFastEndpointsAttribute);
        //var contextAttribute = compilation.GetTypeByMetadataName(AutoFastContextAttribute);
        //var domainValueAttribute = compilation.GetTypeByMetadataName(DomainValueAttribute);

        if (semanticTargetInformations.Where(x => x.Target is AutoFastEndpointsAttribute) is null) return GenerationConfig.Empty;

        //var domainValueDefinitionMetadatas = YieldDomainValueDefinitionMetadata(compilation, semanticTargetInformations).ToImmutableArray();


        var entityClassDeclarations = semanticTargetInformations
            .Where(x => x.Target is AutoFastEndpointsAttribute && x.ClassDeclarationSyntax is not null)
            .Select(x => x.ClassDeclarationSyntax!)
            .ToImmutableArray();

        if (entityClassDeclarations.Length == 0) return GenerationConfig.Empty;

        var entityConfigs = YieldEntityConfig(compilation, entityClassDeclarations, ct).ToImmutableArray();

        var semanticTarget = semanticTargetInformations.FirstOrDefault(x => x.Target == AutoFastContextAttribute);

        if (semanticTarget is null || compilation
            .GetSemanticModel(semanticTarget.ClassDeclarationSyntax!.SyntaxTree)
            .GetDeclaredSymbol(semanticTarget.ClassDeclarationSyntax!) is not INamedTypeSymbol namedTypeSymbol)
        {
            return new GenerationConfig(
                new EntityGenerationConfig(entityConfigs, GetNamespace(entityClassDeclarations.First())));
        }

        return new GenerationConfig(
            new EntityGenerationConfig(entityConfigs, GetNamespace(semanticTarget.ClassDeclarationSyntax)),
            new ContextGenerationConfig(namedTypeSymbol.Name));
    }

    private static IEnumerable<EntityConfigSetup> YieldEntityConfigSetup(
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> entityClassDeclarations)
    {
        foreach (var entityClassDeclaration in entityClassDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(entityClassDeclaration.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(entityClassDeclaration) is not INamedTypeSymbol namedTypeSymbol) continue;

            var endpointsAttributeArguments = GetAutoFastEndpointsAttributeArguments(namedTypeSymbol);

            yield return new EntityConfigSetup(entityClassDeclaration, semanticModel, endpointsAttributeArguments);
        }
    }

    private static AutoFastEndpointsAttributeArguments GetAutoFastEndpointsAttributeArguments(INamedTypeSymbol namedTypeSymbol)
    {
        var endpointsAttribute = namedTypeSymbol.GetAttributes()[0];

        var entityName = endpointsAttribute.ConstructorArguments[0].IsNull is false
            ? endpointsAttribute.ConstructorArguments[0].Value as string
            : GetLastPart(namedTypeSymbol.Name).Replace("Config", "");

        var endpointTarget = (EndpointTargetType)endpointsAttribute.ConstructorArguments[1].Value!;

        return new AutoFastEndpointsAttributeArguments(entityName!, endpointTarget);
    }

    private static IEnumerable<EntityConfig> YieldEntityConfig(
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> entityClassDeclarations,
        CancellationToken ct)
    {
        var entityConfigSetups = YieldEntityConfigSetup(compilation, entityClassDeclarations).ToImmutableArray();

        foreach (var entityConfigSetup in entityConfigSetups)
        {
            ct.ThrowIfCancellationRequested();

            //var entitySubClassDeclarations = entityConfigSetup.ClassDeclarationSyntax
            //    .ChildNodes()
            //    .Where(x => x.Kind() is SyntaxKind.ClassDeclaration)
            //    .Select(x => x as ClassDeclarationSyntax)
            //    .ToImmutableArray();

            //// note: this configuration system should basically be reworked
            //if (entitySubClassDeclarations.Length <= 0
            //    || entitySubClassDeclarations.SingleOrDefault(x => x!.Identifier.Text is "Properties") is not ClassDeclarationSyntax propertiesClass)
            //{
            //    continue;
            //}

            //foreach (var item in propertiesClass.ChildNodes())
            //{
            //    if (item is not ClassDeclarationSyntax domainValueDefinition) continue;

            //    //var ah = domainValueDefinition.BaseList!.Types.ToArray();

            //    //var b = ah[0].ToFullString();

            //    var symbol = entityConfigSetup.SemanticModel.GetDeclaredSymbol(domainValueDefinition);

            //    if (symbol?.BaseType?.TypeArguments.Length > 0)
            //    {
            //        // get name of class and save types
            //        var domainValueTypes = symbol.BaseType.TypeArguments;
            //    }

            //}

            //var types = (propertiesClass.ChildNodes().ToArray()[0] as ClassDeclarationSyntax)!.BaseList.Types.ToArray();

            //var propertiesMembers = entityConfigSetup.SemanticModel.GetDeclaredSymbol(propertiesClass)!.GetMembers();

            //domainValueAttribute.

            var members = entityConfigSetup.SemanticModel
                .GetDeclaredSymbol(entityConfigSetup.ClassDeclarationSyntax)!
                .GetMembers();

            var foreignEntityNames = entityConfigSetups
                .Where(x => x.EndpointsAttributeArguments.EntityName != entityConfigSetup.EndpointsAttributeArguments.EntityName)
                .Select(x => x.EndpointsAttributeArguments.EntityName)
                .ToImmutableArray();

            var propertyMetadatas = YieldPropertyMetadata(compilation, members, foreignEntityNames).ToImmutableArray();

            yield return new EntityConfig(entityConfigSetup.EndpointsAttributeArguments, propertyMetadatas);
        }
    }

    private static IEnumerable<PropertyMetadata> YieldPropertyMetadata(Compilation compilation, ImmutableArray<ISymbol> members, ImmutableArray<string> foreignEntityNames)
    {
        foreach (var member in members)
        {
            if (member is not IPropertySymbol property) continue;

            if (TryGetDomainValueDefinition(compilation, property, out var domainValueDefinition) is false) continue;

            var propertyString = property.ToDisplayString(new SymbolDisplayFormat(
                propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
                memberOptions: SymbolDisplayMemberOptions.IncludeAccessibility));

            var isEnum = domainValueDefinition.EntityType.TypeKind.ToString() == nameof(Enum);

            var aha = (property as INamedTypeSymbol)?.EnumUnderlyingType;

            if (aha != null)
            {

            }

            var relational = GetRelationalEntity(foreignEntityNames, property);
            var entitySource = GetEntityPropertySource(property, propertyString, relational);
            var requestSource = GetRequestPropertySource(property, propertyString, relational, isEnum);

            var attributes = YieldAttributeMetadata(property).ToImmutableArray();

            var requestModelTarget = GetRequestModelTarget(attributes);

            yield return new PropertyMetadata(entitySource, requestSource, property.Name, isEnum, requestModelTarget, attributes, relational);
        }
    }

    private static bool TryGetDomainValueDefinition(Compilation compilation, IPropertySymbol property, out DomainValueDefinition domainValueDefinition)
    {
        domainValueDefinition = default;

        var domainValueType = compilation.GetTypeByMetadataName($"{property.ContainingNamespace}.{property.Name}");

        var success = domainValueType?.BaseType?.TypeArguments.Length > 0;

        if (success)
        {
            var requestType = domainValueType!.BaseType!.TypeArguments[0];

            var entityType = domainValueType.BaseType.TypeArguments.Length == 2
                ? requestType
                : domainValueType.BaseType.TypeArguments[1];

            domainValueDefinition = new DomainValueDefinition(requestType, entityType, property.Name);
        }

        return success;
    }

    private static RequestModelTarget GetRequestModelTarget(ImmutableArray<PropertyAttributeMetadata> attributes)
    {
        if (attributes.Length > 0)
        {
            var attriubteMetadata = attributes.FirstOrDefault(x => x.RequestModelTarget is not null);

            if (attriubteMetadata.RequestModelTarget.HasValue)
            {
                return attriubteMetadata.RequestModelTarget.Value;
            }
        }

        return RequestModelTarget.CreateCommand | RequestModelTarget.ModifyCommand | RequestModelTarget.QueryRequest;
    }

    // note: domainvalue does not support nullable generic params

    //private static string GetTypeKind(IPropertySymbol property)
    //{
    //    if (property.NullableAnnotation is NullableAnnotation.Annotated)
    //    {
    //        if (property.Type is INamedTypeSymbol { TypeArguments.Length: > 0 } namedTypeSymbol)
    //        {
    //            return namedTypeSymbol.TypeArguments[0].TypeKind.ToString();
    //        }
    //    }

    //    return property.Type.TypeKind.ToString();
    //}

    private static string GetRequestPropertySource(IPropertySymbol property, string propertyString, PropertyRelational? relational, bool isEnum)
    {
        // note: define type seperately across requests and entity
        var type = relational switch
        {
            null or
            { RelationalType: RelationalType.ShadowToMany or RelationalType.ShadowToOne } => isEnum ? property.Type.ToDisplayString() : "string?",
            { RelationalType: RelationalType.ToMany } => $"ICollection<{relational.Value.ForeignEntityName}>",
            { RelationalType: RelationalType.ToOne } => relational.Value.ForeignEntityName,
            _ => "object"
        };

        var source = propertyString.Insert(propertyString.IndexOf(' '), $" {type}");

        // note: temporary way of checking types
        //       nullable?
        if (type.Contains("Identifier"))
        {
            if (type.Contains("ICollection"))
            {
                return propertyString.Insert(propertyString.IndexOf(' '), " IEnumerable<string>?");
            }

            return propertyString.Insert(propertyString.IndexOf(' '), " string?");
        }

        return source;
    }

    private static string GetEntityPropertySource(IPropertySymbol property, string propertyString, PropertyRelational? relational)
    {
        // note: define type seperately across requests and entity
        var type = relational switch
        {
            null or
            { RelationalType: RelationalType.ShadowToMany or RelationalType.ShadowToOne } => property.Type.ToDisplayString(),
            { RelationalType: RelationalType.ToMany } => $"ICollection<{relational.Value.ForeignEntityName}>",
            { RelationalType: RelationalType.ToOne } => relational.Value.ForeignEntityName,
            _ => "object"
        };

        var entitySource = propertyString.Insert(propertyString.IndexOf(' '), $" {type}");

        return entitySource;
    }


    // note: this method is based on some conventions, i.e that an entity with a one-to-many relation will declare that
    //       with a property of type ICollection<Foo> and with name Foos.
    //       alot of this is temporary.
    private static PropertyRelational? GetRelationalEntity(ImmutableArray<string> foreignEntityNames, IPropertySymbol property)
    {
        foreach (var foreignEntityName in foreignEntityNames)
        {
            if (property.Name.Contains(foreignEntityName) is false) continue;

            if (property.Type.ToDisplayString().Contains("ICollection"))
            {
                if (property.Name == $"{foreignEntityName}s")
                {
                    return new PropertyRelational(foreignEntityName, property.Name, RelationalType.ToMany);
                }

                if (property.Name == $"{foreignEntityName}Ids")
                {
                    return new PropertyRelational(foreignEntityName, property.Name, RelationalType.ShadowToMany);
                }

                // note: we need exact match to be certain, so just continue here.
                continue;
            }

            if (property.Name == foreignEntityName)
            {
                return new PropertyRelational(foreignEntityName, property.Name, RelationalType.ToOne);
            }

            if (property.Name == $"{foreignEntityName}Id")
            {
                return new PropertyRelational(foreignEntityName, property.Name, RelationalType.ShadowToOne);
            }
        }

        return null;
    }

    private static string GetLastPart(string valueToSubstring, char seperator = '.')
    {
        var index = valueToSubstring.LastIndexOf(seperator);

        if (index == -1) return valueToSubstring;

        var lastPart = valueToSubstring.Substring(index, valueToSubstring.Length - index);

        return lastPart;
    }

    private static IEnumerable<PropertyAttributeMetadata> YieldAttributeMetadata(IPropertySymbol propertyMember)
    {
        foreach (var attributeData in propertyMember.GetAttributes())
        {
            if (attributeData.AttributeClass is null) continue;

            var attributeName = GetLastPart(attributeData.AttributeClass.Name).Replace("Attribute", "");

            if (attributeName == "ExcludeRequestModel")
            {
                if (attributeData.ConstructorArguments.Length > 0)
                {
                    yield return new PropertyAttributeMetadata(
                        AttributeType.Custom,
                        attributeName,
                        (RequestModelTarget)attributeData.ConstructorArguments[0].Value!);

                    continue;
                }

                yield return new PropertyAttributeMetadata(AttributeType.Custom, attributeName, RequestModelTarget.None);
                continue;
            }

            yield return new PropertyAttributeMetadata(AttributeType.Default, attributeName);
        }
    }

    private static string GetNamespace(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var nameSpace = string.Empty;

        var potentialNamespaceParent = classDeclarationSyntax.Parent;

        while (potentialNamespaceParent is not null and not NamespaceDeclarationSyntax and not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            nameSpace = namespaceParent.Name.ToString();
            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                {
                    break;
                }

                namespaceParent = parent;
                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
            }
        }

        return nameSpace;
    }



}
