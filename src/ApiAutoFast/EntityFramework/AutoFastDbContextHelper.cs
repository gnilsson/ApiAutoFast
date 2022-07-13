using ApiAutoFast.Descriptive;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;
using System.Reflection;

namespace ApiAutoFast;

public static class AutoFastDbContextHelper
{
    public static Type[] GetEntityTypes<T>()
    {
        return typeof(T)
            .GetProperties()
            .Where(x => x.PropertyType.Name == TypeText.DbSet1)
            .Select(x => x.PropertyType.GenericTypeArguments[0])
            .ToArray();
    }

    public static void UpdateModifiedDateTime(IEnumerable<EntityEntry> entityEntries)
    {
        foreach (var entry in entityEntries.Where(x => x.Entity is IEntity<IIdentifier> && x.State is EntityState.Modified))
        {
            ((IEntity<IIdentifier>)entry.Entity).ModifiedDateTime = DateTime.UtcNow;
        }
    }

    public static void BuildEntities(ModelBuilder modelBuilder, Type[] entityTypes)
    {
        foreach (var entityType in entityTypes)
        {
            var entityMethod = typeof(ModelBuilder)
                .GetMethod(nameof(ModelBuilder.Entity), 1, Type.EmptyTypes)!
                .MakeGenericMethod(new[] { entityType });

            var entityTypeBuilder = (EntityTypeBuilder)entityMethod.Invoke(modelBuilder, null)!;

            var properties = entityType.GetProperties();

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

            { PropertyType.Name: TypeText.SequentialIdentifier } =>
                entityTypeBuilder
                    .Property(propertyInfo.Name)
                    .HasConversion<SequentialIdentifierValueConverter>()
                    .HasValueGenerator<SequentialIdentifierValueGenerator>(),

            { Name: TypeText.CreatedDateTime or TypeText.ModifiedDateTime } =>
                entityTypeBuilder
                    .Property(propertyInfo.Name)
                    .HasDefaultValueSql("getutcdate()"),

            { PropertyType.BaseType.Name: TypeText.DomainValue2 or TypeText.DomainValue3 or TypeText.DomainValue4 } =>
                entityTypeBuilder
                    .Property(propertyInfo.Name)
                    .HasDomainValueConversion(propertyInfo),

            //note: pin this for now.
            //{ PropertyType.BaseType.Name: TypeText.StringDomainValue1 } =>
            //    entityTypeBuilder
            //        .Property(propertyInfo.Name)
            //        .HasStringDomainValueConversion(propertyInfo),

            _ => null!
        };
    }
}

public static class LinqExtensions
{
    public static IQueryable<T> FixQuery<T>(this IQueryable<T> query)
    {
        return query.Provider.CreateQuery<T>(
            new FixQueryVisitor().Visit(query.Expression)
        );
    }

    private class FixQueryVisitor : ExpressionVisitor
    {
        private static readonly MethodInfo _likeMethod = ExtractMethod(() => EF.Functions.Like(string.Empty, string.Empty));
        private static readonly MethodInfo _containsMethod = ExtractMethod(() => EF.Functions.Contains(string.Empty, string.Empty));

        private static MethodInfo ExtractMethod(Expression<Action> expr)
        {
            MethodCallExpression body = (MethodCallExpression)expr.Body;

            return body.Method;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType?.Name == TypeText.StringDomainValue1 && node.Method.Name == TypeText.Method.Contains)
            {
                //EF.Functions.
                //  var mm = Expression.MakeMemberAccess(node.Object!, node.Method.DeclaringType!.GetProperty("EntityValue")!);
                var c = Expression.Convert(node.Object!, typeof(string));
                var b = Expression.Property(c, "EntityValue");
                return Expression.Call(_containsMethod, Expression.Constant(EF.Functions), c, node.Arguments[0]);
            }

            return base.VisitMethodCall(node);
        }
    }
}
