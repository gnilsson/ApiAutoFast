using ApiAutoFast.SourceGenerator.Configuration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ApiAutoFast.SourceGenerator;

internal record EntityConfigSetup
{
    internal EntityConfigSetup(ClassDeclarationSyntax classDeclarationSyntax, SemanticModel semanticModel, AutoFastEntityAttributeArguments endpointsAttributeArguments)
    {
        ClassDeclarationSyntax = classDeclarationSyntax;
        SemanticModel = semanticModel;
        EndpointsAttributeArguments = endpointsAttributeArguments;
    }

    public ClassDeclarationSyntax ClassDeclarationSyntax { get; }
    public SemanticModel SemanticModel { get; }
    public AutoFastEntityAttributeArguments EndpointsAttributeArguments { get; }
}
