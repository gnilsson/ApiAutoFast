using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator;

internal readonly struct GenerationConfig
{
    internal GenerationConfig(EntityGenerationConfig entityGeneration, ContextGenerationConfig? contextGeneration = null)
    {
        ContextGeneration = contextGeneration;
        EntityGeneration = entityGeneration;
    }

    public static readonly GenerationConfig Empty = default;

    public ContextGenerationConfig? ContextGeneration { get; }
    public EntityGenerationConfig? EntityGeneration { get; }
}

internal readonly struct ContextGenerationConfig
{
    internal ContextGenerationConfig(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

internal readonly struct EntityGenerationConfig
{
    internal EntityGenerationConfig(ImmutableArray<EntityConfig> entityConfigs, string @namespace)
    {
        EntityConfigs = entityConfigs;
        Namespace = @namespace;
    }

    public ImmutableArray<EntityConfig> EntityConfigs { get; }
    public string Namespace { get; }
}

internal readonly struct EntityConfig
{
    internal EntityConfig(string name, in ImmutableArray<PropertyMetadata>? propertyMetadatas = null)
    {
        BaseName = name;
        Response = $"{name}Response";
        MappingProfile = $"{name}MappingProfile";
        PropertyMetadatas = propertyMetadatas;
    }

    public readonly string BaseName { get; }
    public readonly string Response { get; }
    public readonly string MappingProfile { get; }
    public readonly ImmutableArray<PropertyMetadata>? PropertyMetadatas { get; }
}

internal readonly struct EndpointConfig
{
    internal EndpointConfig(EntityConfig entityConfig, RequestEndpointPair requestEndpointPair)
    {
        var endpointTarget = requestEndpointPair.EndpointTarget.ToString();
        Name = $"{endpointTarget}{entityConfig.BaseName}Endpoint";
        EntityName = entityConfig.BaseName;
        MappingProfile = entityConfig.MappingProfile;
        Route = requestEndpointPair.EndpointTarget switch
        {
            EndpointTargetType.GetById
            or EndpointTargetType.Update
            or EndpointTargetType.Delete => $"/{entityConfig.BaseName.ToLower()}s/{{id}}",
            _ => $"/{entityConfig.BaseName.ToLower()}s",
        };
        Response = requestEndpointPair.EndpointTarget switch
        {
            EndpointTargetType.Get => $"PaginatedResponse<{entityConfig.Response}>",
            _ => entityConfig.Response,
        };
        Request = $"{entityConfig.BaseName}{requestEndpointPair.RequestModel}";
        RequestEndpointPair = requestEndpointPair;
    }

    public readonly string Name { get; }
    public readonly string Response { get; }
    public readonly string Request { get; }
    public readonly string EntityName { get; }
    public readonly string Route { get; }
    public readonly string MappingProfile { get; }
    public readonly RequestEndpointPair RequestEndpointPair { get; }
}

internal struct RequestEndpointPair
{
    internal RequestEndpointPair(string requestModel, EndpointTargetType endpointTarget, string httpVerb)
    {
        RequestModel = requestModel;
        EndpointTarget = endpointTarget;
        HttpVerb = httpVerb;
    }

    public string RequestModel { get; }
    public EndpointTargetType EndpointTarget { get; }
    public string HttpVerb { get; }
}

internal readonly struct PropertyMetadata
{
    public PropertyMetadata(string source, string name, ImmutableArray<AttributeMetadata>? attributeMetadatas, Relational? relational = null)
    {
        Source = source;
        Name = name;
        AttributeMetadatas = attributeMetadatas;
        Relational = relational;
    }

    public string Source { get; }
    public string Name { get; }
    public ImmutableArray<AttributeMetadata>? AttributeMetadatas { get; }
    public Relational? Relational { get; }
}

internal readonly struct Relational
{
    public Relational(string foreignEntityName, string foreigEntityProperty, RelationalType relationalType)
    {
        ForeignEntityName = foreignEntityName;
        ForeigEntityProperty = foreigEntityProperty;
        RelationalType = relationalType;
    }

    public string ForeignEntityName { get; }
    public string ForeigEntityProperty { get; }
    public RelationalType RelationalType { get; }
}

internal enum RelationalType
{
    ToOne = 0,
    ToMany,
    ToOneHidden,
    ToManyHidden,
}

internal record SemanticTargetInformation
{
    public SemanticTargetInformation(ClassDeclarationSyntax classDeclarationSyntax, string target)
    {
        ClassDeclarationSyntax = classDeclarationSyntax;
        Target = target;
    }

    public ClassDeclarationSyntax? ClassDeclarationSyntax { get; } = default!;
    public string? Target { get; } = default!;
}

internal enum AttributeType
{
    Target = 0,
    Appliable
}

internal readonly struct AttributeMetadata
{
    internal AttributeMetadata(AttributeType attributeType, string name)
    {
        AttributeType = attributeType;
        Name = name;
    }

    public AttributeType AttributeType { get; }
    public string Name { get; }
}

internal enum AttributeModelTargetType
{
    CreateCommand = 0,
    ModifyCommand,
    QueryRequest,
}

internal enum EndpointTargetType
{
    Get = 0,
    GetById,
    Create,
    Update,
    Delete,
}
