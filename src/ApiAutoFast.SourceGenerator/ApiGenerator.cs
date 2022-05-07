//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.CodeAnalysis.Text;
//using System.Collections.Immutable;
//using System.Text;

//namespace ApiAutoFast;

//public readonly struct EntityDbSetsToGenerate
//{
//    public readonly string NameSpace;
//    public readonly List<string> Names;

//    public EntityDbSetsToGenerate(string namsSpace, List<string> names)
//    {
//        NameSpace = namsSpace;
//        Names = names;
//    }
//}

////[Generator]
////public class ApiGenerator : IIncrementalGenerator
////{
////    private const string AutoFastAttribute = "ApiAutoFast.AutoFastAttribute";
////    private const string AutoFastContextAttribute = "ApiAutoFast.AutoFastContextAttribute";
////    private static bool _context = false;
////    private static bool _endpoints = false;

////    public void Initialize(IncrementalGeneratorInitializationContext context)
////    {
////        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
////            "AutoFastAttribute.g.cs",
////            SourceText.From(SourceEmitter.AutoFastAttribute, Encoding.UTF8)));

////        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
////            "AutoFastContextAttribute.g.cs",
////            SourceText.From(SourceEmitter.AutoFastContextAttribute, Encoding.UTF8)));

////        // if build target is not mapster
////        // get the .Models folder & all files that end with Response.g.cs name and statically add to compilation
////        // gen serializeattribute for {name}Response

////        //context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
////        //    "AutoFasTContext.g.cs",
////        //    SourceText.From(SourceEmitter.AutoFastAttribute, Encoding.UTF8)));

////        //context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
////        //    "AutoFastJson.g.cs",
////        //    SourceText.From(SourceEmitter.AutoFastJson, Encoding.UTF8)));

////        IncrementalValuesProvider<ClassDeclarationSyntax[]> classDeclarations = context.SyntaxProvider
////            .CreateSyntaxProvider(
////                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
////                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
////            .Where(static m => m is not null)!;

////        IncrementalValueProvider<(Compilation, ImmutableArray<ClassDeclarationSyntax[]>)> compilationAndEnums = context.CompilationProvider.Combine(classDeclarations.Collect());

////        context.RegisterSourceOutput(compilationAndEnums, static (spc, source) => Execute(source.Item1, source.Item2, spc));
////    }

////    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0;

////    private static ClassDeclarationSyntax[]? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
////    {
////        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

////        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
////        {
////            foreach (var attributeSyntax in attributeListSyntax.Attributes)
////            {
////                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
////                {
////                    continue;
////                }

////                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;

////                var fullName = attributeContainingTypeSymbol.ToDisplayString();


////                if (fullName == AutoFastAttribute)
////                {
////                    _endpoints = true;

////                    var f = classDeclarationSyntax.Parent!.SyntaxTree;

////                    //var f = classDeclarationSyntax.SyntaxTree.FilePath;
////                    //var c = classDeclarationSyntax.GetLocation().SourceTree?.FilePath;
////                    return classDeclarationSyntax
////                        .ChildNodes()
////                        .Where(x => x is ClassDeclarationSyntax)
////                        .Cast<ClassDeclarationSyntax>()
////                        .ToArray();
////                }
////                else if (fullName == AutoFastContextAttribute)
////                {
////                    _context = true;

////                    return new[] { classDeclarationSyntax };
////                }
////            }
////        }

////        return null;
////    }

////    private static EntityDbSetsToGenerate GetTypesToGenerate(Compilation compilation, IEnumerable<ClassDeclarationSyntax[]> enums, CancellationToken ct)
////    {
////        //      compilation.getnam
////        // Create a list to hold our output
////        var names = new List<string>();
////        // Get the semantic representation of our marker attribute
////        INamedTypeSymbol? enumAttribute = compilation.GetTypeByMetadataName(AutoFastAttribute);

////        if (enumAttribute == null)
////        {
////            // If this is null, the compilation couldn't find the marker attribute type
////            // which suggests there's something very wrong! Bail out..
////            return new(); //enumsToGenerate;
////        }

////        var nameSpace = string.Empty;

////        foreach (var enumDeclarationSyntax in enums)
////        {
////            // stop if we're asked to
////            ct.ThrowIfCancellationRequested();

////            nameSpace = GetNamespace(enumDeclarationSyntax.FirstOrDefault());

////            foreach (var item in enumDeclarationSyntax)
////            {
////                var membs = item.Members.Where(x => x.IsKind(SyntaxKind.PropertyDeclaration)).Cast<PropertyDeclarationSyntax>().ToArray();

////                foreach (var ah in membs)
////                {
////                    //var f = ah.Identifier.Kind();
////                    //var ahhh = (ah.Identifier.Parent as PropertyDeclarationSyntax)!;
////                    //var test = GetNamespace(ahhh);

////                    var enumLol = $"{nameSpace}.Enums.{ah.Type}";

////                    var lul = compilation.GetTypeByMetadataName(enumLol);

////                    if (lul is not null)
////                    {

////                    }

////                    //var fffs = ahhh.Identifier;
////                    var abc = ah.Identifier.Text;
////                    var abcd = ah.Identifier.Value;
////                }

////                var children = item.ChildTokens();
////                var ff = item.DescendantTokens();

////                // Get the semantic representation of the enum syntax
////                var semanticModel = compilation.GetSemanticModel(item.SyntaxTree);

////                if (semanticModel.GetDeclaredSymbol(item) is not INamedTypeSymbol enumSymbol)
////                {
////                    // something went wrong, bail out
////                    continue;
////                }

////                // Get the full type name of the enum e.g. Colour,
////                // or OuterClass<T>.Colour if it was nested in a generic type (for example)
////                var name = enumSymbol.ToString();

////                //// Get all the members in the enum
////                //ImmutableArray<ISymbol> enumMembers = enumSymbol.GetMembers();
////                //var members = new List<string>(enumMembers.Length);

////                //// Get all the fields from the enum, and add their name to the list
////                //foreach (ISymbol member in enumMembers)
////                //{
////                //    if (member is IFieldSymbol field && field.ConstantValue is not null)
////                //    {
////                //        members.Add(member.Name);
////                //    }
////                //}

////                // Create an EnumToGenerate for use in the generation phase
////                var arr = name.Split('.');

////                names.Add(string.Join(".", arr.Skip(arr.Length - 2)));
////            }

////        }

////        return new(nameSpace, names);
////    }

////    //[Conditional()]
////    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax[]> classes, SourceProductionContext context)
////    {
////        if (classes.IsDefaultOrEmpty) return;

////        var distinctEnums = classes.Distinct();

////        var entityDbSets = GetTypesToGenerate(compilation, distinctEnums, context.CancellationToken);

////        if (entityDbSets.Names.Count > 0)
////        {
////            var mappingRegisterResult = SourceEmitter.GenerateMappingRegister(entityDbSets);
////            context.AddSource("MappingRegister.g.cs", SourceText.From(mappingRegisterResult, Encoding.UTF8));

////            var dbContextResult = SourceEmitter.GenerateDbContext(entityDbSets);
////            context.AddSource("AutoFastDbContext.g.cs", SourceText.From(dbContextResult, Encoding.UTF8));

////            var ah = Environment.GetEnvironmentVariables();// .GetEnvironmentVariable("ToolsPatha");
////            var f = Environment.GetCommandLineArgs();

////            Console.WriteLine("trying");

////            if (f.Contains("Mapster"))
////            {
////                Console.WriteLine("yup");
////            }

////            if (_endpoints == true)
////            {
////                var mappingProfileResult = SourceEmitter.GenerateMappingProfiles(entityDbSets);
////                context.AddSource("AutoFastMappingProfiles.g.cs", SourceText.From(mappingProfileResult, Encoding.UTF8));

////                var endpointsResult = SourceEmitter.GenerateEndpoints(entityDbSets);
////                context.AddSource("AutoFastEndpoints.g.cs", SourceText.From(endpointsResult, Encoding.UTF8));
////            }


////            // var dbContext = compilation.GetTypeByMetadataName($"{entityDbSets.NameSpace}.AutoFastDbContext");

////            ////// if mapping register exists

////            ////#if
////            ////# endif
////            ///

////        }
////    }

////    static string GetNamespace(PropertyDeclarationSyntax enumDeclarationSyntax)
////    {
////        // determine the namespace the class is declared in, if any
////        string nameSpace = string.Empty;
////        SyntaxNode? potentialNamespaceParent = enumDeclarationSyntax.Parent;
////        while (potentialNamespaceParent != null &&
////               potentialNamespaceParent is not NamespaceDeclarationSyntax
////               && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
////        {
////            potentialNamespaceParent = potentialNamespaceParent.Parent;
////        }

////        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
////        {
////            nameSpace = namespaceParent.Name.ToString();
////            while (true)
////            {
////                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
////                {
////                    break;
////                }

////                namespaceParent = parent;
////                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
////            }
////        }

////        return nameSpace;
////    }

////    static string GetNamespace(ClassDeclarationSyntax enumDeclarationSyntax)
////    {
////        // determine the namespace the class is declared in, if any
////        string nameSpace = string.Empty;
////        SyntaxNode? potentialNamespaceParent = enumDeclarationSyntax.Parent;
////        while (potentialNamespaceParent != null &&
////               potentialNamespaceParent is not NamespaceDeclarationSyntax
////               && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
////        {
////            potentialNamespaceParent = potentialNamespaceParent.Parent;
////        }

////        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
////        {
////            nameSpace = namespaceParent.Name.ToString();
////            while (true)
////            {
////                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
////                {
////                    break;
////                }

////                namespaceParent = parent;
////                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
////            }
////        }

////        return nameSpace;
////    }
////}
