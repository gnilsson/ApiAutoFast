using ApiAutoFast.Descriptive;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace ApiAutoFast;

internal static class PropertyBuilderExtensions
{
    internal static PropertyBuilder HasDomainValueConversion(this PropertyBuilder propertyBuilder, PropertyInfo propertyInfo)
    {
        var requestTypeArgument = propertyInfo.PropertyType.BaseType?.GenericTypeArguments[0];

        var entityTypeArgument = propertyInfo.PropertyType.BaseType!.Name is TypeText.DomainValue2
            ? requestTypeArgument
            : propertyInfo.PropertyType.BaseType?.GenericTypeArguments[1];

        var responseTypeArgument = propertyInfo.PropertyType.BaseType!.Name is TypeText.DomainValue4
            ? propertyInfo.PropertyType.BaseType?.GenericTypeArguments[2]
            : requestTypeArgument;

        var valueConverter = typeof(DomainValueConverter<,,,>).MakeGenericType(requestTypeArgument!, entityTypeArgument!, responseTypeArgument!, propertyInfo.PropertyType);

        propertyBuilder.HasConversion(valueConverter);

        return propertyBuilder;
    }
}
