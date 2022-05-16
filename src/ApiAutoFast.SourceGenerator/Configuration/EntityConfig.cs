﻿using System.Collections.Immutable;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct EntityConfig
{
    internal EntityConfig(AutoFastEndpointsAttributeArguments endpointsAttributeArguments, ImmutableArray<PropertyMetadata> propertyMetadatas)
    {
        BaseName = endpointsAttributeArguments.EntityName;
        Response = $"{endpointsAttributeArguments.EntityName}Response";
        MappingProfile = $"{endpointsAttributeArguments.EntityName}MappingProfile";
        EndpointsAttributeArguments = endpointsAttributeArguments;
        PropertyMetadatas = propertyMetadatas;
    }

    internal readonly string BaseName { get; }
    internal readonly string Response { get; }
    internal readonly string MappingProfile { get; }
    internal readonly AutoFastEndpointsAttributeArguments EndpointsAttributeArguments { get; }
    internal readonly ImmutableArray<PropertyMetadata> PropertyMetadatas { get; }
}
