//HintName: AbcQueryRequest.g.cs

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class AbcQueryRequest
{
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public ApiAutoFast.Sample.Server.Database.ProfessionCategory? Profession { get; set; }
}
