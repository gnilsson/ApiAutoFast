namespace ApiAutoFast;

public class DomainValue<TRequestEntityResponse, TDomain> : DomainValue<TRequestEntityResponse, TRequestEntityResponse, TDomain>
    where TDomain : DomainValue<TRequestEntityResponse, TRequestEntityResponse, TDomain>, new()
{
    // note: can something be done here?
    protected override bool TryValidateRequestConversion(TRequestEntityResponse? requestValue, out TRequestEntityResponse entityValue)
    {
        entityValue = requestValue!;
        return requestValue is not null;
    }


    //public static implicit operator TRequestEntityResponse(DomainValue<TRequestEntityResponse, TDomain> domain) => domain.EntityValue;
}



//public static class LulExtensions
//{
//    public static bool Cont<TDomain>(this DomainValue<string, TDomain> domain, string value) where TDomain : DomainValue<string, TDomain>, new()
//    {
//        return domain.EntityValue.Contains(value);
//    }
//}