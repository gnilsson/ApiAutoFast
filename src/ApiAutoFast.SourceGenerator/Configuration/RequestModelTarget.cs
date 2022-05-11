namespace ApiAutoFast.SourceGenerator;

[Flags]
internal enum RequestModelTarget
{
    None = 0,
    CreateCommand = 1,
    ModifyCommand = 2,
    QueryRequest = 4,
    GetByIdRequest = 8,
    DeleteCommand = 16,
}
