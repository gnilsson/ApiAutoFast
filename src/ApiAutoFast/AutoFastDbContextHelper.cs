using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Reflection;

namespace ApiAutoFast;

public static class AutoFastDbContextHelper
{
    private const string ComplexString = nameof(ApiAutoFast.DefaultString);
    private const string Identifier = nameof(ApiAutoFast.Identifier);
    private const string ComplexOf2 = "ComplexOf`2";
    private const string CreatedDateTime = nameof(IEntity.CreatedDateTime);
    private const string ModifiedDateTime = nameof(IEntity.ModifiedDateTime);


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
        Func<PropertyBuilder> _ = propertyInfo switch
        {
            { PropertyType.BaseType.Name: ComplexString } or { PropertyType.IsEnum: true } => () =>
            {
                return entityTypeBuilder
                        .Property(propertyInfo.Name)
                        .HasConversion<string>();
            }
            ,
            { PropertyType.Name: Identifier } => () =>
            {
                return entityTypeBuilder
                        .Property(propertyInfo.Name)
                        .HasConversion<IdentifierValueConverter>()
                        .HasValueGenerator<IdentifierValueGenerator>();
            }
            ,

            { PropertyType.BaseType.Name: ComplexOf2 } => () =>
            {
                if (ComplexPropertyValueConverterContainer.GetOne(propertyInfo.PropertyType.Name) is ComplexPropertyValueConverter valueConverter)
                {
                    entityTypeBuilder
                        .Property(propertyInfo.Name)
                        .HasConversion(valueConverter.ValueConverterType);
                }

                // note: no registered valueConverter, throw
                return null!;
            }
            ,
            { PropertyType.Name: CreatedDateTime or ModifiedDateTime } => () =>
            {
                return entityTypeBuilder
                        .Property(propertyInfo.Name)
                        .HasDefaultValueSql("getutcdate()");
            }
            ,
            _ => default!
        };
    }
}
