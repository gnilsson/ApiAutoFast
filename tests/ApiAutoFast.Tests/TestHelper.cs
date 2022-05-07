using ApiAutoFast.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VerifyXunit;

namespace ApiAutoFast.Tests;

public static class TestHelper
{
    public static Task Verify(string source)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

        IEnumerable<PortableExecutableReference> references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
          //  MetadataReference.CreateFromFile(typeof(AutoFastDbContext).Assembly.Location)
        };

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: references);//GetMetadataReferences()

        var generator = new ApiGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGenerators(compilation);

        return Verifier
            .Verify(driver)
            .UseDirectory("Snapshots");
    }

    private static IEnumerable<MetadataReference> GetMetadataReferences()
    {
        var dotNetAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
        if (!string.IsNullOrEmpty(dotNetAssemblyPath))
        {
            ImmutableArray<MetadataReference> references = ImmutableArray.Create<MetadataReference>(
            // .NET assemblies are finicky and need to be loaded in a special way.
            MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "mscorlib.dll")),
            MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.dll")),
            MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Core.dll")),
            MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Private.CoreLib.dll")),
            MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Runtime.dll")));
           // MetadataReference.CreateFromFile(typeof(AutoFastDbContext).Assembly.Location));

            return references;
        }

        return Enumerable.Empty<MetadataReference>();
    }
}
