using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
using ApiAutoFast.SourceGenerator.Emitters;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace ApiAutoFast.SourceGenerator;

[Generator]
public class ApiGenerator : IIncrementalGenerator
{
    private static readonly ImmutableArray<RequestEndpointPair> _requestEndpointPairs;

    static ApiGenerator()
    {
        _requestEndpointPairs = ImmutableArray.Create(new RequestEndpointPair[]
        {
            new (RequestModelTarget.GetByIdRequest, PropertyTarget.None,            EndpointTargetType.GetById, "HttpVerb.Get"),
            new (RequestModelTarget.DeleteCommand,  PropertyTarget.None,            EndpointTargetType.Delete,  "HttpVerb.Delete"),
            new (RequestModelTarget.ModifyCommand,  PropertyTarget.ModifyCommand,   EndpointTargetType.Update,  "HttpVerb.Put"),
            new (RequestModelTarget.CreateCommand,  PropertyTarget.CreateCommand,   EndpointTargetType.Create,  "HttpVerb.Post"),
            new (RequestModelTarget.QueryRequest,   PropertyTarget.QueryRequest,    EndpointTargetType.Get,     "HttpVerb.Get"),
        });
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("EndpointTargetEnum.g.cs", SourceText.From(EmbeddedSourceEmitter.EndpointTargetEnum, Encoding.UTF8));
            ctx.AddSource("IdTypeEnum.g.cs", SourceText.From(EmbeddedSourceEmitter.IdTypeEnum, Encoding.UTF8));
            ctx.AddSource("AutoFastEntityAttribute.g.cs", SourceText.From(EmbeddedSourceEmitter.AutoFastEntityAttribute, Encoding.UTF8));
            ctx.AddSource("AutoFastContextAttribute.g.cs", SourceText.From(EmbeddedSourceEmitter.AutoFastContextAttribute, Encoding.UTF8));
            ctx.AddSource("AutoFastEndpointAttribute.g.cs", SourceText.From(EmbeddedSourceEmitter.AutoFastEndpointAttribute, Encoding.UTF8));
            ctx.AddSource("RequestModelTargetEnum.g.cs", SourceText.From(EmbeddedSourceEmitter.RequestModelTargetEnum, Encoding.UTF8));
            ctx.AddSource("ExcludeRequestModelAttribute.g.cs", SourceText.From(EmbeddedSourceEmitter.ExcludeRequestModelAttribute, Encoding.UTF8));
            ctx.AddSource("IncludeInCommandAttribute.g.cs", SourceText.From(EmbeddedSourceEmitter.IncludeInCommandAttribute, Encoding.UTF8));
        });

        IncrementalValuesProvider<SemanticTargetInformation> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => Parser.IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => Parser.GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)!;

        IncrementalValueProvider<(Compilation Compilation, ImmutableArray<SemanticTargetInformation> SemanticTargetInformations)> compilationSemanticTargetInformations =
            context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationSemanticTargetInformations, static (spc, source) => Execute(source.Compilation, source.SemanticTargetInformations, spc));
    }

    private static void Execute(Compilation compilation, ImmutableArray<SemanticTargetInformation> semanticTargets, SourceProductionContext context)
    {
        if (semanticTargets.IsDefaultOrEmpty) return;

        var generationConfig = Parser.GetGenerationConfig(compilation, semanticTargets, context.CancellationToken);

        if (generationConfig is null or { EntityGeneration.EntityConfigs.Length: <= 0 }) return;

        var entityGenerationConfig = generationConfig.Value.EntityGeneration!.Value;

        var sb = new StringBuilder();

        foreach (var entityConfig in entityGenerationConfig.EntityConfigs)
        {
            var entityResult = DefaultSourceEmitter.EmitEntityModel(sb, entityGenerationConfig.Namespace, entityConfig);
            context.AddSource($"{entityConfig.BaseName}.g.cs", SourceText.From(entityResult, Encoding.UTF8));
        }

        if (generationConfig!.Value.ContextGeneration.HasValue is false) return;

        var contextConfig = generationConfig!.Value.ContextGeneration.Value;

        var dbContextResult = DefaultSourceEmitter.EmitDbContext(sb, contextConfig, entityGenerationConfig);
        context.AddSource($"{contextConfig.Name}.g.cs", SourceText.From(dbContextResult, Encoding.UTF8));

        foreach (var entityConfig in entityGenerationConfig.EntityConfigs)
        {
            var simplifiedResponseResult = MapperSourceEmitter.EmitSimplifiedResponseModel(sb, entityGenerationConfig.Namespace, entityConfig);
            context.AddSource($"{entityConfig.Response}Simplified.g.cs", SourceText.From(simplifiedResponseResult, Encoding.UTF8));

            var responseResult = MapperSourceEmitter.EmitResponseModel(sb, entityGenerationConfig.Namespace, entityConfig);
            context.AddSource($"{entityConfig.Response}.g.cs", SourceText.From(responseResult, Encoding.UTF8));

            var foreginConfigs = entityGenerationConfig.EntityConfigs.Where(x => x.BaseName != entityConfig.BaseName).ToImmutableArray();
            var mapperResult = MapperSourceEmitter.EmitMapper(sb, entityGenerationConfig.Namespace, entityConfig, foreginConfigs);
            context.AddSource($"{entityConfig.BaseName}Mapper.g.cs", SourceText.From(mapperResult, Encoding.UTF8));

            foreach (var requestEndpointPair in _requestEndpointPairs)
            {
                var requestModelsResult = DefaultSourceEmitter.EmitRequestModel(sb, entityGenerationConfig.Namespace, entityConfig, requestEndpointPair.RequestModel, requestEndpointPair.PropertyTarget);
                context.AddSource($"{entityConfig.BaseName}{requestEndpointPair.RequestModel}.g.cs", SourceText.From(requestModelsResult, Encoding.UTF8));
            }

            var mappingProfileResult = DefaultSourceEmitter.EmitMappingProfile(sb, entityGenerationConfig.Namespace, entityConfig);
            context.AddSource($"{entityConfig.MappingProfile}.g.cs", SourceText.From(mappingProfileResult, Encoding.UTF8));

            foreach (var requestEndpointPair in _requestEndpointPairs)
            {
                if (entityConfig.EndpointsAttributeArguments.EndpointTargetType.HasFlag(requestEndpointPair.EndpointTarget))
                {
                    var endpoint = $"{requestEndpointPair.EndpointTarget}{entityConfig.BaseName}Endpoint";
                    var isTargetedEndpoint = entityGenerationConfig.TargetedEndpointNames.HasValue && entityGenerationConfig.TargetedEndpointNames.Contains(endpoint);
                    var endpointConfig = new EndpointConfig(endpoint, entityConfig, requestEndpointPair, isTargetedEndpoint, entityConfig.RelationalNavigationNames, entityConfig.StringEntityProperties, contextConfig.Name);

                    var requestModelsResult = EndpointSourceEmitter.EmitEndpoint(sb, entityGenerationConfig.Namespace, endpointConfig);
                    context.AddSource($"{endpointConfig.Endpoint}.g.cs", SourceText.From(requestModelsResult, Encoding.UTF8));
                }
            }
        }
    }
}
