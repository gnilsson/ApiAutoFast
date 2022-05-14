using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Reflection;

namespace ApiAutoFast;

public static class AutoFastDbContextHelper
{
    private const string Identifier = nameof(ApiAutoFast.Identifier);
    private const string CreatedDateTime = nameof(IEntity.CreatedDateTime);
    private const string ModifiedDateTime = nameof(IEntity.ModifiedDateTime);
    private const string DomainValue2 = "DomainValue`2";
    private const string DomainValue3 = "DomainValue`3";

    public static Type[] GetEntityTypes<T>()
    {
        return typeof(T)
            .GetProperties()
            .Where(x => x.PropertyType.Name == "DbSet`1")
            .Select(x => x.PropertyType.GenericTypeArguments[0])
            .ToArray();
    }

    public static void UpdateModifiedDateTime(IEnumerable<EntityEntry> entityEntries)
    {
        foreach (var entry in entityEntries.Where(x => x.Entity is IEntity && x.State is EntityState.Modified))
        {
            ((IEntity)entry.Entity).ModifiedDateTime = DateTime.UtcNow;
        }
    }

    public static void BuildEntities(ModelBuilder modelBuilder, Type[] entityTypes)
    {
        foreach (var entityType in entityTypes)
        {
            var properties = entityType.GetProperties();

            var entityMethod = typeof(ModelBuilder)
                .GetMethod(nameof(ModelBuilder.Entity), 1, Type.EmptyTypes)!
                .MakeGenericMethod(new[] { entityType });

            var entityTypeBuilder = (EntityTypeBuilder)entityMethod.Invoke(modelBuilder, null)!;

            foreach (var property in properties)
            {
                SetPropertyFactories(entityTypeBuilder, property);
            }
        }
    }

    private static void SetPropertyFactories(EntityTypeBuilder entityTypeBuilder, PropertyInfo propertyInfo)
    {
        _ = propertyInfo switch
        {
            //{ PropertyType.BaseType.Name: DomainValue2 } or { PropertyType.IsEnum: true } =>
            //    entityTypeBuilder
            //        .Property(propertyInfo.Name)
            //        .HasConversion<string>(),

            { PropertyType.Name: Identifier } =>
                 entityTypeBuilder
                    .Property(propertyInfo.Name)
                    .HasConversion<IdentifierValueConverter>()
                    .HasValueGenerator<IdentifierValueGenerator>(),

            { PropertyType.BaseType.Name: DomainValue2 } => AttemptAddValueConverter(entityTypeBuilder, propertyInfo, 0),

            { PropertyType.BaseType.Name: DomainValue3 } => AttemptAddValueConverter(entityTypeBuilder, propertyInfo, 1),

            { Name: CreatedDateTime or ModifiedDateTime } =>
                entityTypeBuilder
                    .Property(propertyInfo.Name)
                    .HasDefaultValueSql("getutcdate()"),

            _ => null!
        };
    }

    private static PropertyBuilder AttemptAddValueConverter(EntityTypeBuilder entityTypeBuilder, PropertyInfo propertyInfo, int entityTypeGenericTypeParamPosition)
    {
        var entityTypeArgument = propertyInfo.PropertyType.BaseType?.GenericTypeArguments[entityTypeGenericTypeParamPosition].Name;

        if (entityTypeArgument is not null && DomainValueConverterContainer.Values.TryGetValue(entityTypeArgument, out var converterType))
        {
            var valueConverter = converterType.MakeGenericType(propertyInfo.PropertyType);

            entityTypeBuilder
                .Property(propertyInfo.Name)
                .HasConversion(valueConverter);
        }

        // note: no registered valueConverter, throw
        return null!;
    }
}
