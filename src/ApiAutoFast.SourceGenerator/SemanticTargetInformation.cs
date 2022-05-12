using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ApiAutoFast.SourceGenerator;

internal sealed record SemanticTargetInformation
{
    public SemanticTargetInformation(ClassDeclarationSyntax classDeclarationSyntax, string target)
    {
        ClassDeclarationSyntax = classDeclarationSyntax;
        Target = target;
    }

    internal ClassDeclarationSyntax? ClassDeclarationSyntax { get; } = default!;
    internal string? Target { get; } = default!;
}
