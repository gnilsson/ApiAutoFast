namespace ApiAutoFast;

public static class PaginatedResponseBuilder
{
    public static Paginated<TResponse> Build<TResponse>(
        IUriService uriService,
        string route,
        PaginationQuery paginationQuery,
        ICollection<TResponse> response,
        int? total)
    {
        var nextPage = paginationQuery!.PageNumber >= 1
            ? uriService.GetUri(route, new PaginationQuery
            {
                PageNumber = paginationQuery.PageNumber + 1,
                PageSize = paginationQuery.PageSize
            }).ToString()
            : null;

        var previousPage = paginationQuery.PageNumber - 1 >= 1
            ? uriService.GetUri(route, new PaginationQuery
            {
                PageNumber = paginationQuery.PageNumber - 1,
                PageSize = paginationQuery.PageSize
            }).ToString()
            : null;

        return new Paginated<TResponse>
        {
            Data = response,
            PageNumber = paginationQuery.PageNumber >= 1 ? paginationQuery.PageNumber : null,
            PageSize = paginationQuery.PageSize > response.Count ? response.Count : paginationQuery.PageSize >= 1 ? paginationQuery.PageSize : null,
            NextPage = total > paginationQuery.PageSize ? nextPage : null,
            PreviousPage = previousPage,
            Total = total,
        };
    }
}
