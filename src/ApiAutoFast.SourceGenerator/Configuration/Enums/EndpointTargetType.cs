namespace ApiAutoFast.SourceGenerator.Configuration.Enums;

[Flags]
internal enum EndpointTargetType
{
    None = 0,
    Get = 1,
    GetById = 2,
    Create = 4,
    Update = 8,
    Delete = 16,
    All = Get | Create | Update | GetById | Delete
}
