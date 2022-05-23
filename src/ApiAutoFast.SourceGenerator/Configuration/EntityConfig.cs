using ApiAutoFast.SourceGenerator.Configuration.Enums;
using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct EntityConfig
{
    internal EntityConfig(AutoFastEndpointsAttributeArguments endpointsAttributeArguments, ImmutableArray<PropertyMetadata> propertyMetadatas, ImmutableArray<string> relationalNavigationNames)
    {
        BaseName = endpointsAttributeArguments.EntityName;
        Response = $"{endpointsAttributeArguments.EntityName}Response";
        MappingProfile = $"{endpointsAttributeArguments.EntityName}MappingProfile";
        CreateCommand = $"{endpointsAttributeArguments.EntityName}{nameof(RequestModelTarget.CreateCommand)}";
        ModifyCommand = $"{endpointsAttributeArguments.EntityName}{nameof(RequestModelTarget.ModifyCommand)}";
        EndpointsAttributeArguments = endpointsAttributeArguments;
        PropertyMetadatas = propertyMetadatas;
        RelationalNavigationNames = relationalNavigationNames;
    }

    internal readonly string BaseName { get; }
    internal readonly string Response { get; }
    internal readonly string MappingProfile { get; }
    internal readonly string CreateCommand { get; }
    internal readonly string ModifyCommand { get; }
    internal readonly AutoFastEndpointsAttributeArguments EndpointsAttributeArguments { get; }
    internal readonly ImmutableArray<PropertyMetadata> PropertyMetadatas { get; }
    internal readonly ImmutableArray<string> RelationalNavigationNames { get; }
}
