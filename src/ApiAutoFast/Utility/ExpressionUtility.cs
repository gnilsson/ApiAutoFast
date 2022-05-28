using System.Linq.Expressions;

namespace ApiAutoFast.Utility;

public static class ExpressionUtility
{
    public delegate object EmptyConstructorDelegate();

    public static EmptyConstructorDelegate CreateEmptyConstructor(Type type)
    {
        return Expression.Lambda<EmptyConstructorDelegate>(
            Expression.New(
                type.GetConstructor(Type.EmptyTypes)!)).Compile();
    }

    public static Expression<Func<TEntity, bool>> AndAlso<TEntity>(
        Expression<Func<TEntity, bool>> expr1,
        Expression<Func<TEntity, bool>> expr2)
        where TEntity : class
    {
        ParameterExpression param = expr1.Parameters[0];
        if (ReferenceEquals(param, expr2.Parameters[0]))
        {
            return Expression.Lambda<Func<TEntity, bool>>(
                Expression.AndAlso(expr1.Body, expr2.Body), param);
        }
        return Expression.Lambda<Func<TEntity, bool>>(
            Expression.AndAlso(
                expr1.Body,
                Expression.Invoke(expr2, param)), param);
    }
}
