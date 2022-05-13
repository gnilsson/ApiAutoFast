﻿using ApiAutoFast.SourceGenerator.Configuration;
using ApiAutoFast.SourceGenerator.Configuration.Enums;
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
            new (RequestModelTarget.GetByIdRequest, EndpointTargetType.GetById, "Http.GET"),
            new (RequestModelTarget.DeleteCommand,  EndpointTargetType.Delete,  "Http.DELETE"),
            new (RequestModelTarget.ModifyCommand,  EndpointTargetType.Update,  "Http.PUT"),
            new (RequestModelTarget.CreateCommand,  EndpointTargetType.Create,  "Http.POST"),
            new (RequestModelTarget.QueryRequest,   EndpointTargetType.Get,     "Http.GET"),
        };
    }

    private static readonly RequestEndpointPair[] _requestEndpointPairs;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource("EndpointTargetEnum.g.cs", SourceText.From(EmbeddedSourceEmitter.EndpointTargetEnum, Encoding.UTF8));
            ctx.AddSource("AutoFastEndpointsAttribute.g.cs", SourceText.From(EmbeddedSourceEmitter.AutoFastEndpointsAttribute, Encoding.UTF8));
            ctx.AddSource("AutoFastContextAttribute.g.cs", SourceText.From(EmbeddedSourceEmitter.AutoFastContextAttribute, Encoding.UTF8));
            ctx.AddSource("RequestModelTargetEnum.g.cs", SourceText.From(EmbeddedSourceEmitter.RequestModelTargetEnum, Encoding.UTF8));
            ctx.AddSource("ExcludeRequestModelAttribute.g.cs", SourceText.From(EmbeddedSourceEmitter.ExcludeRequestModelAttribute, Encoding.UTF8));
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

        // note: it would be great if we could make the check on what files the compiler has generated
        //       maybe check should be made on mapster files?
        if (generationConfig!.Value.ContextGeneration.HasValue is false) return;

        var contextConfig = generationConfig!.Value.ContextGeneration.Value;

        var dbContextResult = SourceEmitter.EmitDbContext(sb, contextConfig, entityGenerationConfig);
        context.AddSource($"{contextConfig.Name}.g.cs", SourceText.From(dbContextResult, Encoding.UTF8));

        foreach (var entityConfig in entityGenerationConfig.EntityConfigs)
        {
            foreach (var requestEndpointPair in _requestEndpointPairs)
            {
                var requestModelsResult = SourceEmitter.EmitRequestModel(sb, entityGenerationConfig.Namespace, entityConfig, requestEndpointPair.RequestModel);
                context.AddSource($"{entityConfig.BaseName}{requestEndpointPair.RequestModel}.g.cs", SourceText.From(requestModelsResult, Encoding.UTF8));
            }

            // note: with empty partial responses pre mapster & context attribute existing, error begins here.
            //       if filepath.exists models?
            var mappingProfileResult = SourceEmitter.EmitMappingProfile(sb, entityGenerationConfig.Namespace, entityConfig);
            context.AddSource($"{entityConfig.MappingProfile}.g.cs", SourceText.From(mappingProfileResult, Encoding.UTF8));

            foreach (var requestEndpointPair in _requestEndpointPairs)
            {
                if (entityConfig.EndpointsAttributeArguments.EndpointTargetType.HasFlag(requestEndpointPair.EndpointTarget))
                {
                    var endpointConfig = new EndpointConfig(entityConfig, requestEndpointPair);
                    var requestModelsResult = SourceEmitter.EmitEndpoint(sb, entityGenerationConfig.Namespace, endpointConfig, contextConfig.Name);
                    context.AddSource($"{endpointConfig.Name}.g.cs", SourceText.From(requestModelsResult, Encoding.UTF8));
                }
            }
        }
    }
}
