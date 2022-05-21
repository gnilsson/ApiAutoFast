using System.Linq.Expressions;

namespace ApiAutoFast.Utilities;

internal static class ExpressionUtilities
{
    public delegate object EmptyConstructorDelegate();

    public static EmptyConstructorDelegate CreateEmptyConstructor(Type type)
    {
        return Expression.Lambda<EmptyConstructorDelegate>(
            Expression.New(
                type.GetConstructor(Type.EmptyTypes)!)).Compile();
    }
}
