using FastEndpoints;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace ApiAutoFast;

//public interface IEntityConverter<TEntityValue, TThis>
//{
//    public TEntityValue? EntityValue { get; protected set; }
//    public TThis ConvertFromEntity(TEntityValue entityValue);
//}

//public static class DomainConverter<TEntityRequestValue, TThis> where TThis : DomainValue<TEntityRequestValue, TEntityRequestValue, TThis>, new()
//{
//    static DomainConverter()
//    {
//        FactoryContainer = new();
//    }

//    public static Dictionary<string, Func<TThis>> FactoryContainer { get; }

//    public static TThis ConvertFromEntity(TEntityRequestValue entityValue)
//    {
//        var complex = FactoryContainer[nameof(TEntityRequestValue)]();
//        complex.EntityValue = entityValue;
//        return complex;
//    }
//}

public class DateTimeValueConverter<TDomain> : ValueConverter<TDomain, DateTime> where TDomain : DomainValue<string, DateTime, TDomain>, new()
{
    public DateTimeValueConverter() : base(
    s => s.EntityValue,
    t => DomainConverter<string, DateTime, TDomain>.ConvertFromEntity(t))
    { }
}

public class StringValueConverter<TDomain> : ValueConverter<TDomain, string> where TDomain : DomainValue<string, string, TDomain>, new()
{
    public StringValueConverter() : base(
    s => s.EntityValue!,
    t => DomainConverter<string, string, TDomain>.ConvertFromEntity(t))
    { }
}

internal static class DomainValueConverterContainer
{
    internal static Dictionary<string, Type> Values { get; } = new()
    {
        [nameof(DateTime)] = typeof(DateTimeValueConverter<>),
        [nameof(String)] = typeof(StringValueConverter<>),
    };
}

public static class DomainConverter<TRequestValue, TEntityValue, TThis> where TThis : DomainValue<TRequestValue, TEntityValue, TThis>, new()
{
    static DomainConverter()
    {
        FactoryContainer = new();
    }

    public static Dictionary<string, Func<TThis>> FactoryContainer { get; }

    public static TThis ConvertFromEntity(TEntityValue entityValue)
    {
        var complex = FactoryContainer[nameof(TEntityValue)]();
        complex.EntityValue = entityValue;
        return complex;
    }
}

public class DomainValue<TRequestValue, TEntityValue, TThis> where TThis : DomainValue<TRequestValue, TEntityValue, TThis>, new()
{
    static DomainValue()
    {
        var ctor = typeof(TThis)
            .GetTypeInfo()
            .DeclaredConstructors
            .First();

        var argsExp = Array.Empty<Expression>();
        var newExp = Expression.New(ctor, argsExp);
        var lambda = Expression.Lambda(typeof(Func<TThis>), newExp);

        _factory = (Func<TThis>)lambda.Compile();

        DomainConverter<TRequestValue, TEntityValue, TThis>.FactoryContainer.Add(nameof(TEntityValue), _factory);
    }

    protected static readonly Func<TThis> _factory;

    public TRequestValue? RequestValue { get; protected set; }
    public TEntityValue? EntityValue { get; internal set; }
    protected virtual string? MessageOnFailedValidation { get; }

    protected virtual bool TryValidateRequestConvertion(TRequestValue requestValue, out TEntityValue entityValue)
    {
        throw new NotImplementedException($"DomainValue`3 needs an overriden {nameof(TryValidateRequestConvertion)} method.");
    }

    //protected void AddEntityFrameworkConverter<TConverter>()
    //{
    //    DomainValueConverterContainer.Values.Add(typeof(TConverter));
    //}

    protected virtual void Configure()
    {

    }

    public static TThis ConvertFromRequest(TRequestValue request, Action<string, string> addError)
    {
        var complex = _factory();
        complex.RequestValue = request;

        if (complex.TryValidateRequestConvertion(request, out var entityValue))
        {
            complex.EntityValue = entityValue;
            return complex;
        }

        addError(nameof(TThis), complex.MessageOnFailedValidation ?? "Error when converting request.");
        return default!;
    }

    //public TThis ConvertFromEntity(TEntityValue entityValue)
    //{
    //    var complex = _factory();
    //    complex.EntityValue = entityValue;
    //    return complex;
    //}
}

public class DomainValue<TEntityRequestValue, TThis> : DomainValue<TEntityRequestValue, TEntityRequestValue, TThis>
    where TThis : DomainValue<TEntityRequestValue, TEntityRequestValue, TThis>, new()
{
    //static DomainValue()
    //{
    //    DomainConverter<TEntityRequestValue, TThis>.FactoryContainer.Add(nameof(TEntityRequestValue), _factory);
    //}

    protected override bool TryValidateRequestConvertion(TEntityRequestValue requestValue, out TEntityRequestValue entityValue)
    {
        var success = requestValue is not null;
        entityValue = success ? requestValue : default!;
        return success;
    }

    public override string ToString() => EntityValue!.ToString()!;
}
