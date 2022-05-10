using Microsoft.AspNetCore.WebUtilities;

namespace ApiAutoFast;

public class UriService : IUriService
{
    private const string PAGE_NUMBER = "pageNumber";
    private const string PAGE_SIZE = "pageSize";

    private readonly string _baseUri;

    public UriService(string baseUri) => _baseUri = baseUri;

    public Uri GetByIdUri(string requestRoute, string id)
    {
        return new Uri(_baseUri + requestRoute.Replace(requestRoute[requestRoute.IndexOf("{")..], id));
    }

    public Uri GetUri(string requestRoute, IPaginateableRequest? paginationData = null)
    {
        var uri = new Uri(_baseUri + requestRoute);

        if (paginationData is null) return uri;

        var uriString = uri.ToString();

        QueryHelpers.AddQueryString(uriString, PAGE_NUMBER, paginationData.PageNumber.ToString());
        QueryHelpers.AddQueryString(uriString, PAGE_SIZE, paginationData.PageSize.ToString());

        return new Uri(uriString);
    }
}
