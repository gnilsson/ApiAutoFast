namespace ApiAutoFast;

public class DomainValue<TRequestEntityResponse, TDomain>
    : DomainValue<TRequestEntityResponse, TRequestEntityResponse, TDomain>
    where TDomain : DomainValue<TRequestEntityResponse, TRequestEntityResponse, TDomain>, new()
{
    // note: can something be done here?
    protected override bool TryValidateRequestConversion(TRequestEntityResponse? requestValue, out TRequestEntityResponse entityValue)
    {
        entityValue = requestValue!;
        return requestValue is not null;
    }

    //public static implicit operator TEntityRequestValue(DomainValue<TEntityRequestValue, TEntityRequestValue, TDomain> domain)
    //{
    //    return domain.EntityValue;
    //}

    protected override TRequestEntityResponse ToResponse()
    {
        return EntityValue;
    }
}
