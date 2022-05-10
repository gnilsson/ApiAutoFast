namespace ApiAutoFast;

public interface IUriService
{
    public Uri GetByIdUri(string requestRoute, string id);
    public Uri GetUri(string requestRoute, IPaginateableRequest? paginationData = null);
}
