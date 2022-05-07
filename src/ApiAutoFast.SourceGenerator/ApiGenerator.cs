using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace ApiAutoFast.SourceGenerator;

[Generator]
public class ApiGenerator : IIncrementalGenerator
{
    static ApiGenerator()
    {
        _requestEndpointPairs = new RequestEndpointPair[]
        {
            new($"{nameof(EndpointTargetType.GetById)}Request", EndpointTargetType.GetById, "Http.GET"),
            new($"{nameof(EndpointTargetType.Delete)}Request",EndpointTargetType.Delete, "Http.DELETE"),
            new(nameof(AttributeModelTargetType.ModifyCommand),EndpointTargetType.Update, "Http.PUT"),
            new(nameof(AttributeModelTargetType.CreateCommand),EndpointTargetType.Create, "Http.POST"),
            new(nameof(AttributeModelTargetType.QueryRequest),EndpointTargetType.Get, "Http.GET"),
        };
    }

    private static readonly RequestEndpointPair[] _requestEndpointPairs;
    private static readonly string[] _propertyTargetAttributeNames = Enum.GetNames(typeof(AttributeModelTargetType));

    private const string AutoFastEndpointsAttribute = "ApiAutoFast.AutoFastEndpointsAttribute";
    private const string AutoFastContextAttribute = "ApiAutoFast.AutoFastContextAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "AutoFastEndpointsAttribute.g.cs",
            SourceText.From(SourceEmitter.AutoFastEndpointsAttribute, Encoding.UTF8)));

        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "AutoFastContextAttribute.g.cs",
            SourceText.From(SourceEmitter.AutoFastContextAttribute, Encoding.UTF8)));

        IncrementalValuesProvider<SemanticTargetInformation> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m.HasValue)
            .Select(static (m, _) => m!.Value);

        IncrementalValueProvider<(Compilation, ImmutableArray<SemanticTargetInformation>)> compilationAndEnums = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndEnums, static (spc, source) => Execute(source.Item1, source.Item2, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0;

    private static SemanticTargetInformation? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
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

                if (fullName is AutoFastEndpointsAttribute or AutoFastContextAttribute)
                {
                    return new(classDeclarationSyntax, fullName);
                }
            }
        }

        return null;
    }

    private static void Execute(Compilation compilation, ImmutableArray<SemanticTargetInformation> semanticTargets, SourceProductionContext context)
    {
        if (semanticTargets.IsDefaultOrEmpty) return;

        var distinctSemanticTargets = semanticTargets.Distinct();

        var generationConfig = GetGenerationConfig(compilation, distinctSemanticTargets, context.CancellationToken);

        if (generationConfig is null) return;

        if ((generationConfig!.Value.EntityGeneration?.EntityConfigs.Length) <= 0) return;

        var entityGenerationConfig = generationConfig.Value.EntityGeneration!.Value;

        foreach (var entityConfig in entityGenerationConfig.EntityConfigs)
        {
            var entityResult = SourceEmitter.EmitEntityModels(entityGenerationConfig.Namespace, entityConfig);
            context.AddSource($"{entityConfig.BaseName}.g.cs", SourceText.From(entityResult, Encoding.UTF8));
        }

        var mappingRegisterResult = SourceEmitter.EmitMappingRegister(entityGenerationConfig);
        context.AddSource("MappingRegister.g.cs", SourceText.From(mappingRegisterResult, Encoding.UTF8));

        if (!generationConfig!.Value.ContextGeneration.HasValue) return;

        var contextConfig = generationConfig!.Value.ContextGeneration.Value;

        var dbContextResult = SourceEmitter.EmitDbContext(contextConfig, entityGenerationConfig);
        context.AddSource($"{contextConfig.Name}.g.cs", SourceText.From(dbContextResult, Encoding.UTF8));

        foreach (var entityConfig in entityGenerationConfig.EntityConfigs)
        {
            foreach (var requestEndpointPair in _requestEndpointPairs)
            {
                var requestModelsResult = SourceEmitter.EmitModelTarget(entityGenerationConfig.Namespace, entityConfig, requestEndpointPair.RequestModel);
                context.AddSource($"{entityConfig.BaseName}{requestEndpointPair.RequestModel}.g.cs", SourceText.From(requestModelsResult, Encoding.UTF8));
            }

            var mappingProfileResult = SourceEmitter.EmitMappingProfile(entityGenerationConfig.Namespace, entityConfig);
            context.AddSource($"{entityConfig.MappingProfile}.g.cs", SourceText.From(mappingProfileResult, Encoding.UTF8));

            foreach (var requestEndpointPair in _requestEndpointPairs)
            {
                var endpointConfig = new EndpointConfig(entityConfig, requestEndpointPair);
                var requestModelsResult = SourceEmitter.EmitEndpoint(entityGenerationConfig.Namespace, endpointConfig, contextConfig.Name);
                context.AddSource($"{endpointConfig.Name}.g.cs", SourceText.From(requestModelsResult, Encoding.UTF8));
            }
        }
    }

    private static GenerationConfig? GetGenerationConfig(Compilation compilation, IEnumerable<SemanticTargetInformation> semanticTargetInformations, CancellationToken ct)
    {
        var endpointAttribute = compilation.GetTypeByMetadataName(AutoFastEndpointsAttribute);
        var contextAttribute = compilation.GetTypeByMetadataName(AutoFastContextAttribute);

        if (endpointAttribute is null || contextAttribute is null || semanticTargetInformations.Where(x => x.Target == AutoFastEndpointsAttribute) is null)
        {
            return GenerationConfig.Empty;
        }

        var entityEndpoints = new List<EntityConfig>();
        var contextGenerationConfig = default(ContextGenerationConfig?);
        var @namespace = string.Empty;

        foreach (var semanticTargetInformation in semanticTargetInformations)
        {
            ct.ThrowIfCancellationRequested();

            var semanticModel = compilation.GetSemanticModel(semanticTargetInformation.ClassDeclarationSyntax!.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(semanticTargetInformation.ClassDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol)
            {
                continue;
            }

            var tempName = namedTypeSymbol.ToString().Split('.');
            var name = string.Join(".", tempName.Skip(tempName.Length - 1));

            if (semanticTargetInformation.Target == AutoFastContextAttribute)
            {
                contextGenerationConfig = new ContextGenerationConfig(name);

                continue;
            }

            name = name.Replace("Config", "");
            @namespace = GetNamespace(semanticTargetInformation.ClassDeclarationSyntax);

            var entityConfigClassDeclarationSyntaxes = semanticTargetInformation.ClassDeclarationSyntax
                .ChildNodes()
                .Where(x => x.Kind() == SyntaxKind.ClassDeclaration)?
                .Cast<ClassDeclarationSyntax>();

            if (entityConfigClassDeclarationSyntaxes is null)
            {
                // throw invalid config
                continue;
            }

            var propertiesClass = entityConfigClassDeclarationSyntaxes.SingleOrDefault(x => x.Identifier.Text == "Properties");

            if (propertiesClass is null)
            {
                // throw?
                entityEndpoints.Add(new EntityConfig(name));

                continue;
            }

            var members = semanticModel.GetDeclaredSymbol(propertiesClass)!.GetMembers();

            var propertyMetadatas = YieldPropertyMetadatas(members).ToImmutableArray();

            entityEndpoints.Add(new EntityConfig(name, propertyMetadatas));
        }

        return new GenerationConfig(
            contextGenerationConfig,
            new EntityGenerationConfig(entityEndpoints.ToImmutableArray(), @namespace));
    }

    private static IEnumerable<PropertyMetadata> YieldPropertyMetadatas(ImmutableArray<ISymbol> members)
    {
        foreach (var member in members)
        {
            if (member is not IPropertySymbol property) continue;

            var propertyString = property.ToDisplayString(new SymbolDisplayFormat(
                propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
                memberOptions: SymbolDisplayMemberOptions.IncludeAccessibility));

            var source = propertyString.Insert(propertyString.IndexOf(' '), $" {property.Type.ToDisplayString()}");

            var attributes = YieldAttributeMetadatas(property).ToImmutableArray();

            yield return new PropertyMetadata(source, property.Name, attributes);

        }
    }

    private static IEnumerable<AttributeMetadata> YieldAttributeMetadatas(IPropertySymbol propertyMember)
    {
        foreach (var attributeData in propertyMember.GetAttributes())
        {
            if (attributeData.AttributeClass is null) continue;

            var attributeName = attributeData.AttributeClass.Name.Split('.').Last().Replace("Attribute", "");

            if (_propertyTargetAttributeNames.Contains(attributeName))
            {
                yield return new(AttributeType.Target, attributeName);
                continue;
            }

            yield return new(AttributeType.Appliable, attributeName);
            continue;
        }
    }

    private static string GetNamespace(ClassDeclarationSyntax enumDeclarationSyntax)
    {
        var nameSpace = string.Empty;

        var potentialNamespaceParent = enumDeclarationSyntax.Parent;

        while (potentialNamespaceParent != null &&
               potentialNamespaceParent is not NamespaceDeclarationSyntax
               && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
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
