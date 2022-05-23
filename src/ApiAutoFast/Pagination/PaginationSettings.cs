namespace ApiAutoFast;

public readonly struct PaginationSettings
{
    public PaginationSettings()
    { }

    public int DefaultSize { get; } = 20;
    public int MaxSize { get; } = 50;
}
