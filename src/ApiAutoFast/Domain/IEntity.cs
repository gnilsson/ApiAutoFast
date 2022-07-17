namespace ApiAutoFast;


public interface IEntity<TId> : ITimestamp where TId : IIdentifier
{
    public TId Id { get; set; }
}
