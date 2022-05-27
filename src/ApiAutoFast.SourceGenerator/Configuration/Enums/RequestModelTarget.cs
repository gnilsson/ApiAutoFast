namespace ApiAutoFast.SourceGenerator.Configuration.Enums;

[Flags]
internal enum RequestModelTarget
{
    None = 0,
    CreateCommand = 1,
    ModifyCommand = 2,
    QueryRequest = 4,
    GetByIdRequest = 8,
    DeleteCommand = 16,
    Defaults = CreateCommand | ModifyCommand | QueryRequest,
}
