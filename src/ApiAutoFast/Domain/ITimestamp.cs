namespace ApiAutoFast;

public interface ITimestamp
{
    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }
}
