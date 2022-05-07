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
        if (propertyInfo.Name.EndsWith("Id"))
        {
            entityTypeBuilder.Property(propertyInfo.Name)
                .HasConversion<IdentifierValueConverter>()
                .HasValueGenerator<IdentifierValueGenerator>();

            return;
        }

        if (propertyInfo.PropertyType.IsEnum)
        {
            entityTypeBuilder.Property(propertyInfo.Name).HasConversion<string>();

            return;
        }

        if (propertyInfo.Name.EndsWith("DateTime"))
        {
            entityTypeBuilder.Property(propertyInfo.Name).HasDefaultValueSql("getutcdate()");

            return;
        }
    }
}
