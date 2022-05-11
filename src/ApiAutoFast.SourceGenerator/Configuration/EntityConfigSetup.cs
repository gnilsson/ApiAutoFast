using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ApiAutoFast.SourceGenerator;

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



[Flags]
internal enum RequestModelTarget
{
    None = 0,
    CreateCommand = 1,
    ModifyCommand = 2,
    QueryRequest = 4,
    GetByIdRequest = 8,
    DeleteCommand = 16,
}
