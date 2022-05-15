namespace ApiAutoFast;

public class DomainValue<TEntityRequestValue, TThis> : DomainValue<TEntityRequestValue, TEntityRequestValue, TThis>
    where TThis : DomainValue<TEntityRequestValue, TEntityRequestValue, TThis>, new()
{
    // note: can something be done here?
    protected override bool TryValidateRequestConversion(TEntityRequestValue? requestValue, out TEntityRequestValue entityValue)
    {
        entityValue = requestValue!;
        return requestValue is not null;
    }
}
