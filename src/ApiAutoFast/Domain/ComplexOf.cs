using FastEndpoints;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

namespace ApiAutoFast;

//public class ComplexStringValueConverter<T> : ValueConverter<string, T> where T : DefaultString
//{
//    public ComplexStringValueConverter() : base(
//        s => (T)DefaultString.ConvertFrom(s),
//        t => t.EntityValue!)
//    { }
//}

//public interface IComplexOf<TRequest, TEntity, TThis> where TThis : IComplexOf<TRequest, TEntity, TThis>
//{
//    public string? RequestValue { get; protected set; }
//    public string? EntityValue { get; protected set; }

//    bool TryConvertValidate(TRequest item, out TEntity entityValue);

//    bool TryConvertFrom(TRequest item, out TThis thisValue);
//}


public class DefaultString : ComplexOf<DefaultString>
{
    protected override bool TryConvertValidate(string requestValue, out string entityValue)
    {
        if (string.IsNullOrEmpty(requestValue))
        {
            entityValue = default!;
            return false;
        }

        entityValue = requestValue;
        return true;
    }
}


public class ComplexOf<TThis> where TThis : ComplexOf<TThis>, new()
{
    static ComplexOf()
    {
        var ctor = typeof(TThis)
            .GetTypeInfo()
            .DeclaredConstructors
            .First();

        var argsExp = Array.Empty<Expression>();
        var newExp = Expression.New(ctor, argsExp);
        var lambda = Expression.Lambda(typeof(Func<TThis>), newExp);

        _factory = (Func<TThis>)lambda.Compile();
    }

    private static readonly Func<TThis> _factory;

    public string? RequestValue { get; protected set; }
    public string? EntityValue { get; protected set; }

    protected virtual bool TryConvertValidate(string requestValue, out string entityValue)
    {
        if (string.IsNullOrEmpty(requestValue))
        {
            entityValue = default!;
            return false;
        }

        entityValue = requestValue;
        return true;
    }

    public static bool TryConvertFrom(string item, out TThis thisValue)
    {
        var complex = _factory();
        complex.RequestValue = item;

        thisValue = default!;

        if (complex.TryConvertValidate(item, out var aha))
        {
            thisValue = complex;
            complex.EntityValue = aha;
        }

        return thisValue is not null;
    }

    public static TThis ConvertFrom(string entityValue)
    {
        var complex = _factory();
        complex.EntityValue = entityValue;
        return complex;
    }

    public override string ToString() => EntityValue ?? RequestValue!;
}

public class ComplexOf<TEntityValue, TThis> where TThis : ComplexOf<TEntityValue, TThis>, new()
{
    static ComplexOf()
    {
        var ctor = typeof(TThis)
            .GetTypeInfo()
            .DeclaredConstructors
            .First();

        var argsExp = Array.Empty<Expression>();
        var newExp = Expression.New(ctor, argsExp);
        var lambda = Expression.Lambda(typeof(Func<TThis>), newExp);

        _factory = (Func<TThis>)lambda.Compile();
    }

    private static readonly Func<TThis> _factory;

    public string? RequestValue { get; protected set; }
    public TEntityValue? EntityValue { get; protected set; }
    protected virtual string? MessageOnFailedValidation { get; }
    protected virtual bool TryValidate() => true;

    protected virtual void Configure()
    {

    }

    protected virtual bool TryConvertValidate(string item, out TEntityValue entityValue)
    {
        // throw not impl
        entityValue = default!;
        return false;
    }

    public static bool TryConvertFrom(string request, out TThis complex)
    {
        complex = _factory();
        complex.RequestValue = request;

        if (complex.TryConvertValidate(request, out var entityValue))
        {
            complex.EntityValue = entityValue;
        }

        return complex is not null;
    }

    public static TThis ConvertFrom(string request, Action<string, string> addError)
    {
        var complex = _factory();
        complex.RequestValue = request;

        if (complex.TryConvertValidate(request, out var entityValue))
        {
            complex.EntityValue = entityValue;
            return complex;
        }

        addError(nameof(TThis), complex.MessageOnFailedValidation ?? "Error when converting request.");
        return default!;
    }

    public static TThis ConvertFrom(string request)
    {
        var complex = _factory();
        complex.RequestValue = request;

        if (complex.TryConvertValidate(request, out var entityValue))
        {
            complex.EntityValue = entityValue;
            return complex;
        }

        return default!;
    }

    public static TThis ConvertFrom(TEntityValue entityValue)
    {
        var complex = _factory();
        complex.EntityValue = entityValue;
        return complex;
    }

    protected virtual string ErrorMessage { get; } = "Could not convert.";
}

public record ComplexPropertyValueConverter(Type ComplexPropertyType, Type ValueConverterType);

public static class ComplexPropertyValueConverterContainer
{
    static ComplexPropertyValueConverterContainer()
    {
        var valueConverterTargets = new List<ComplexPropertyValueConverter>();
        Add = (asd) => valueConverterTargets.Add(asd);
        Get = () => valueConverterTargets.ToImmutableArray();
        GetOne = (name) => valueConverterTargets.FirstOrDefault(x => x.ComplexPropertyType.Name == name);
    }

    //  private static readonly ICollection<ComplexPropertyValueConverter> _valueConverterTargets;
    public static Action<ComplexPropertyValueConverter> Add { get; }
    internal static Func<ImmutableArray<ComplexPropertyValueConverter>> Get { get; }
    internal static Func<string, ComplexPropertyValueConverter?> GetOne { get; }
}

