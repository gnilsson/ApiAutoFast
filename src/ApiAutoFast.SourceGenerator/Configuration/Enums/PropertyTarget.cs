namespace ApiAutoFast.SourceGenerator.Configuration.Enums;

[Flags]
internal enum PropertyTarget
{
    None = 0,
    Entity = 1,
    CreateCommand = 2,
    ModifyCommand = 4,
    QueryRequest = 8,
}
