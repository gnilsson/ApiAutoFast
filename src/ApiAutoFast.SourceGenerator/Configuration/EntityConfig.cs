using ApiAutoFast.SourceGenerator.Configuration.Enums;
using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct EntityConfig
{
    internal EntityConfig(
        AutoFastEntityAttributeArguments endpointsAttributeArguments,
        PropertyConfig propertyConfig,
        ImmutableArray<string> relationalNavigationNames,
        ImmutableArray<string> stringEntityProperties)
    {
        BaseName = endpointsAttributeArguments.EntityName;
        Response = $"{endpointsAttributeArguments.EntityName}Response";
        MappingProfile = $"{endpointsAttributeArguments.EntityName}MappingProfile";
        CreateCommand = $"{endpointsAttributeArguments.EntityName}{nameof(RequestModelTarget.CreateCommand)}";
        ModifyCommand = $"{endpointsAttributeArguments.EntityName}{nameof(RequestModelTarget.ModifyCommand)}";
        EndpointsAttributeArguments = endpointsAttributeArguments;
        PropertyConfig = propertyConfig;
        RelationalNavigationNames = relationalNavigationNames;
        StringEntityProperties = stringEntityProperties;
    }

    internal readonly string BaseName { get; }
    internal readonly string Response { get; }
    internal readonly string MappingProfile { get; }
    internal readonly string CreateCommand { get; }
    internal readonly string ModifyCommand { get; }
    internal readonly AutoFastEntityAttributeArguments EndpointsAttributeArguments { get; }
    internal readonly PropertyConfig PropertyConfig { get; }
    internal readonly ImmutableArray<string> RelationalNavigationNames { get; }
    internal readonly ImmutableArray<string> StringEntityProperties { get; }
}
