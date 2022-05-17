using ApiAutoFast.Descriptive;

namespace ApiAutoFast;

public class DomainValue<TRequest, TEntity, TDomain> : DomainValue<TRequest, TEntity, TRequest, TDomain>
    where TDomain : DomainValue<TRequest, TEntity, TRequest, TDomain>, new()
{
    protected override bool TryValidateRequestConversion(TRequest? requestValue, out TEntity entityValue)
    {
        throw new NotImplementedException($"{TypeText.DomainValue3} needs an overriden {nameof(TryValidateRequestConversion)} method.");
    }

    protected override TRequest ToResponse()
    {
        throw new NotImplementedException($"{TypeText.DomainValue3} needs an overriden {nameof(ToResponse)} method.");
    }
}
