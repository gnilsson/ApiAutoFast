namespace ApiAutoFast;

public interface IEntity<T> : ITimestamp where T : IIdentifier
{
    public T Id { get; set; }
}
