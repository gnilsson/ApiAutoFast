﻿//HintName: AuthorQueryRequest.g.cs

using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public class AuthorQueryRequest
{
    public string? CreatedDateTime { get; set; }
    public string? ModifiedDateTime { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ApiAutoFast.Sample.Server.Database.ProfessionCategory? Profession { get; set; }
}
