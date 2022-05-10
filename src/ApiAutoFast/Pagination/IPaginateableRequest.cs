namespace ApiAutoFast;

public interface IPaginateableRequest
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
