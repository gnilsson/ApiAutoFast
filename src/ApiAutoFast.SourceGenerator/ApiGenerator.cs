using Microsoft.CodeAnalysis;
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
            new ($"{nameof(EndpointTargetType.GetById)}Request", EndpointTargetType.GetById, "Http.GET"),
            new ($"{nameof(EndpointTargetType.Delete)}Command",  EndpointTargetType.Delete,  "Http.DELETE"),
            new (nameof(AttributeModelTargetType.ModifyCommand), EndpointTargetType.Update,  "Http.PUT"),
            new (nameof(AttributeModelTargetType.CreateCommand), EndpointTargetType.Create,  "Http.POST"),
            new (nameof(AttributeModelTargetType.QueryRequest),  EndpointTargetType.Get,     "Http.GET"),
        };
    }

    private static readonly RequestEndpointPair[] _requestEndpointPairs;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("AutoFastEndpointsAttribute.g.cs", SourceText.From(SourceEmitter.AutoFastEndpointsAttribute, Encoding.UTF8));
            ctx.AddSource("AutoFastContextAttribute.g.cs", SourceText.From(SourceEmitter.AutoFastContextAttribute, Encoding.UTF8));
        });

        IncrementalValuesProvider<SemanticTargetInformation> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => Parser.IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => Parser.GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)!;

        IncrementalValueProvider<(Compilation Compilation, ImmutableArray<SemanticTargetInformation> SemanticTargetInformations)> compilationAndEnums =
            context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndEnums, static (spc, source) => Execute(source.Compilation, source.SemanticTargetInformations, spc));
    }

    private static void Execute(Compilation compilation, ImmutableArray<SemanticTargetInformation> semanticTargets, SourceProductionContext context)
    {
        if (semanticTargets.IsDefaultOrEmpty) return;

        var distinctSemanticTargets = semanticTargets.Distinct();

        var generationConfig = Parser.GetGenerationConfig(compilation, distinctSemanticTargets, context.CancellationToken);

        if (generationConfig is null or { EntityGeneration.EntityConfigs.Length: <= 0 }) return;

        var entityGenerationConfig = generationConfig.Value.EntityGeneration!.Value;

        var sb = new StringBuilder();

        foreach (var entityConfig in entityGenerationConfig.EntityConfigs)
        {
            var entityResult = SourceEmitter.EmitEntityModels(sb, entityGenerationConfig.Namespace, entityConfig);
            context.AddSource($"{entityConfig.BaseName}.g.cs", SourceText.From(entityResult, Encoding.UTF8));
        }

        var mappingRegisterResult = SourceEmitter.EmitMappingRegister(sb, entityGenerationConfig);
        context.AddSource("MappingRegister.g.cs", SourceText.From(mappingRegisterResult, Encoding.UTF8));

        if (!generationConfig!.Value.ContextGeneration.HasValue) return;

        var contextConfig = generationConfig!.Value.ContextGeneration.Value;

        var dbContextResult = SourceEmitter.EmitDbContext(sb, contextConfig, entityGenerationConfig);
        context.AddSource($"{contextConfig.Name}.g.cs", SourceText.From(dbContextResult, Encoding.UTF8));

        foreach (var entityConfig in entityGenerationConfig.EntityConfigs)
        {
            foreach (var requestEndpointPair in _requestEndpointPairs)
            {
                var requestModelsResult = SourceEmitter.EmitRequestModelTarget(sb, entityGenerationConfig.Namespace, entityConfig, requestEndpointPair.RequestModel);
                context.AddSource($"{entityConfig.BaseName}{requestEndpointPair.RequestModel}.g.cs", SourceText.From(requestModelsResult, Encoding.UTF8));
            }

            var mappingProfileResult = SourceEmitter.EmitMappingProfile(sb, entityGenerationConfig.Namespace, entityConfig);
            context.AddSource($"{entityConfig.MappingProfile}.g.cs", SourceText.From(mappingProfileResult, Encoding.UTF8));

            foreach (var requestEndpointPair in _requestEndpointPairs)
            {
                var endpointConfig = new EndpointConfig(entityConfig, requestEndpointPair);
                var requestModelsResult = SourceEmitter.EmitEndpoint(sb, entityGenerationConfig.Namespace, endpointConfig, contextConfig.Name);
                context.AddSource($"{endpointConfig.Name}.g.cs", SourceText.From(requestModelsResult, Encoding.UTF8));
            }
        }
    }
}
