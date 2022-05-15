using ApiAutoFast.Descriptive;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace ApiAutoFast;

public static class AutoFastDbContextHelper
{
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
            { PropertyType.Name: TypeText.Identifier } =>
                 entityTypeBuilder
                    .Property(propertyInfo.Name)
                    .HasConversion<IdentifierValueConverter>()
                    .HasValueGenerator<IdentifierValueGenerator>(),

            { Name: TypeText.CreatedDateTime or TypeText.ModifiedDateTime } =>
                entityTypeBuilder
                    .Property(propertyInfo.Name)
                    .HasDefaultValueSql("getutcdate()"),

            { PropertyType.BaseType.Name: TypeText.DomainValue2 or TypeText.DomainValue3 } =>
                entityTypeBuilder
                    .Property(propertyInfo.Name)
                    .HasDomainValueConversion(propertyInfo),

            _ => null!
        };
    }
}
