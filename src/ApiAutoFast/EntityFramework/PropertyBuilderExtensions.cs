using ApiAutoFast.Descriptive;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace ApiAutoFast;

internal static class PropertyBuilderExtensions
{
    public static PropertyBuilder HasDomainValueConversion(this PropertyBuilder propertyBuilder, PropertyInfo propertyInfo)
    {
        var requestTypeArgument = propertyInfo.PropertyType.BaseType?.GenericTypeArguments[0];

        var entityTypeArgument = propertyInfo.PropertyType.BaseType!.Name is TypeText.DomainValue2
            ? requestTypeArgument
            : propertyInfo.PropertyType.BaseType?.GenericTypeArguments[1];

        var valueConverter = typeof(DomainValueConverter<,,>).MakeGenericType(requestTypeArgument!, entityTypeArgument!, propertyInfo.PropertyType);

        propertyBuilder.HasConversion(valueConverter);

        return propertyBuilder;
    }
}
