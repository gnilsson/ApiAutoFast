namespace ApiAutoFast;

//public class Paginated<TResponse>
//{
//    public IEnumerable<TResponse>? Data { get; init; }

//    public int? PageNumber { get; init; }

//    public int? PageSize { get; init; }

//    public string? NextPage { get; init; }

//    public string? PreviousPage { get; init; }

//    public int? Total { get; init; }

//    public IAsyncEnumerable<TResponse>? Data2 { get; init; }
//}

public class Paginated<TResponse>
{
    public IAsyncEnumerable<TResponse>? Data { get; init; }
}
