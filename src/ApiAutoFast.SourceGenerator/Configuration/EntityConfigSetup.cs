using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal record EntityConfigSetup
{
    internal EntityConfigSetup(ClassDeclarationSyntax classDeclarationSyntax, SemanticModel semanticModel, string name)
    {
        ClassDeclarationSyntax = classDeclarationSyntax;
        SemanticModel = semanticModel;
        Name = name;
    }

    public ClassDeclarationSyntax ClassDeclarationSyntax { get; }
    public SemanticModel SemanticModel { get; }
    public string Name { get; set; }
}
