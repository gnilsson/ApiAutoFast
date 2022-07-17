using ApiAutoFast.Descriptive;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace ApiAutoFast.EntityFramework;

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
