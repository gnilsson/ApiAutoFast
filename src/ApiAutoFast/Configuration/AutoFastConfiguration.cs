using ApiAutoFast.Domain;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace ApiAutoFast;

public abstract class AutoFastConfiguration
{
    public abstract void Configure();

    public static void AddValidationErrorMessage(string domainValueName, string errorMessage)
    {
        ValidationErrorMessageContainer.Values.Add(domainValueName, errorMessage);
    }

    public static void AddValidationErrorMessage<TDomain>(string errorMessage)
    {
        ValidationErrorMessageContainer.Values.Add(typeof(TDomain).Name, errorMessage);
    }
}


public class PropertySetup<T> where T : class
{
    private readonly T _setup;

    public PropertySetup(T setup)
    {
        _setup = setup;
    }

    public PropertySetup<T> WithValidationErrorMessage(string errorMessage)
    {
        ValidationErrorMessageContainer.Values.Add(typeof(T).Name, errorMessage);

        return this;
    }

    public PropertySetup<T> WithDomainValueTypes<TRequest, TEntity, TResponse>(Action<IDomainValueTypesSetup<TRequest, TEntity, TResponse>>? options = null)
    {
        return this;
    }

    public PropertySetup<T> WithDomainValueTypes<TRequest, TEntity>(Action<IDomainValueTypesSetup<TRequest, TEntity, TRequest>>? options = null)
    {
        return this;
    }

    public PropertySetup<T> WithDomainValueTypes<TRequestEntityResponse>(Action<IDomainValueTypesSetup<TRequestEntityResponse, TRequestEntityResponse, TRequestEntityResponse>>? options = null)
    {
        return this;
    }


}

public interface IDomainValueTypesSetup<TRequest, TEntity, TResponse>
{
    Try<TRequest?, TEntity> TryValidateRequestConversion { get; set; }
}

public delegate bool Try<TIn, TOut>(TIn? input, out TOut output);

public interface IPropertySetup : ISetup
{

}

public interface IEntitySetup : ISetup
{

}

public interface ISetup
{
    void Setup();
}



public class EntitySetup<T> where T : class
{
    private readonly T _setup;

    public EntitySetup(T setup)
    {
        _setup = setup;
    }

    public EntitySetup<T> WithProperty<U>(Action<IEntityPropertyOptions>? options = null)
    {
        return this;
    }

    public EntitySetup<T> WithRelationToOne<U>(Action<IEntityPropertyOptions>? options = null)
    {
        return this;
    }

    public EntitySetup<T> WithRelationToMany<U>(Action<IEntityPropertyOptions>? options = null)
    {
        return this;
    }
}


public interface IEntityPropertyOptions
{
    public IEntityPropertyOptions WithAttribute<T>(params object[] args) where T : Attribute
    {
        return this;
    }

    public IEntityPropertyOptions WithAttribute<T>(string fieldName) where T : Attribute
    {
        return this;
    }
}

public static class SetupExtensions
{
    public static EntitySetup<T> EntitySetup<T>(this T setup) where T : class, IEntitySetup
    {
        return new EntitySetup<T>(setup);
    }

    public static PropertySetup<T> PropertySetup<T>(this T setup) where T : class
    {
        return new PropertySetup<T>(setup);
    }
}
