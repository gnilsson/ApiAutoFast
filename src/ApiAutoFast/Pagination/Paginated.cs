namespace ApiAutoFast;

public class Paginated<TResponse>
{
    public IAsyncEnumerable<TResponse>? Data { get; init; }
    public int Size { get; init; }
    public string? ReferenceId { get; init; }
}
