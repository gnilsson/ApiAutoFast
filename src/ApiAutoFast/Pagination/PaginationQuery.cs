namespace ApiAutoFast;

public record PaginationQuery : IPaginateableRequest
{
    public PaginationQuery()
    { }

    public PaginationQuery(IPaginateableRequest request, PaginationSettings settings)
    {
        PageNumber = request?.PageNumber < 1
            ? _defaultPageNumber
            : request!.PageNumber;

        PageSize = request.PageSize < 1
            ? settings.DefaultPageSize
            : request.PageSize > settings.MaxPageSize ? settings.MaxPageSize
            : request.PageSize;
    }

    private readonly int _defaultPageNumber = 1;

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}
