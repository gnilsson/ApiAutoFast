namespace ApiAutoFast;

public class Paginated<TResponse>
{
    public IEnumerable<TResponse>? Data { get; init; }
}
