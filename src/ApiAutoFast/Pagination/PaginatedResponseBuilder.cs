namespace ApiAutoFast;

public class PaginatedResponseBuilder
{
    public static Paginated<TResponse> Build<TResponse>(
IUriService uriService,
QueryRequest request,
ICollection<TResponse> response,
int? total)
    {
        var pagination = request.PaginationQuery;

        var nextPage = pagination!.PageNumber >= 1 ? uriService.GetUri(request.RequestRoute!, new PaginationQuery
        {
            PageNumber = pagination.PageNumber + 1,
            PageSize = pagination.PageSize
        }).ToString() : null;

        var previousPage = pagination.PageNumber - 1 >= 1 ? uriService.GetUri(request.RequestRoute!, new PaginationQuery
        {
            PageNumber = pagination.PageNumber - 1,
            PageSize = pagination.PageSize
        }).ToString() : null;

        return new PaginateableResponse<TResponse>
        {
            Data = response,
            PageNumber = pagination.PageNumber >= 1 ? pagination.PageNumber : null,
            PageSize = pagination.PageSize > response.Count ? response.Count : pagination.PageSize >= 1 ? pagination.PageSize : null,
            NextPage = total > pagination.PageSize ? nextPage : null,
            PreviousPage = previousPage,
            Total = total,
            Errors = request.Errors
        };
    }
}
