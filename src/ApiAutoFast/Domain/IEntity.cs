namespace ApiAutoFast;

//public interface IEntity
//{
//    public Identifier Id { get; set; }
//    public DateTime CreatedDateTime { get; set; }
//    public DateTime ModifiedDateTime { get; set; }
//}

public interface IEntity<T> where T : IIdentifier
{
    public T Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }
}