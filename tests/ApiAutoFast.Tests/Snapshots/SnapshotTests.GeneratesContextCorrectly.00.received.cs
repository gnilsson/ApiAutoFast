//HintName: Author.g.cs

using ApiAutoFast;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public class Author : IEntity
{
    public Author()
    {
    }

    public Identifier Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ApiAutoFast.Sample.Server.Database.ProfessionCategory? Profession { get; set; }
}
