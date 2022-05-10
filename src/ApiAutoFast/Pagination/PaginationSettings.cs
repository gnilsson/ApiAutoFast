namespace ApiAutoFast;

public readonly struct PaginationSettings
{
    public PaginationSettings()
    { }

    public int DefaultPageSize { get; } = 20;
    public int MaxPageSize { get; } = 50;
}
