namespace ApiAutoFast;

public class PaginatedResponse<TResponse>
{
    public IEnumerable<TResponse>? Data { get; init; }
}
