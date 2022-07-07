using ApiAutoFast.SourceGenerator.Configuration.Enums;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct EntityConfig
{
    internal EntityConfig(
        AutoFastEndpointsAttributeArguments endpointsAttributeArguments,
        ImmutableArray<string> relationalNavigationNames,
        PropertyConfig propertyConfig,
        ImmutableArray<string> stringEntityProperties)
    {
        BaseName = endpointsAttributeArguments.EntityName;
        Response = $"{endpointsAttributeArguments.EntityName}Response";
        MappingProfile = $"{endpointsAttributeArguments.EntityName}MappingProfile";
        CreateCommand = $"{endpointsAttributeArguments.EntityName}{nameof(RequestModelTarget.CreateCommand)}";
        ModifyCommand = $"{endpointsAttributeArguments.EntityName}{nameof(RequestModelTarget.ModifyCommand)}";
        EndpointsAttributeArguments = endpointsAttributeArguments;
        RelationalNavigationNames = relationalNavigationNames;
        PropertyConfig = propertyConfig;
        StringEntityProperties = stringEntityProperties;
    }

    internal readonly string BaseName { get; }
    internal readonly string Response { get; }
    internal readonly string MappingProfile { get; }
    internal readonly string CreateCommand { get; }
    internal readonly string ModifyCommand { get; }
    internal readonly AutoFastEndpointsAttributeArguments EndpointsAttributeArguments { get; }
    internal readonly ImmutableArray<string> RelationalNavigationNames { get; }
    internal readonly PropertyConfig PropertyConfig { get; }
    internal readonly ImmutableArray<string> StringEntityProperties { get; }
}
