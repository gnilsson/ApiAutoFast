
using ApiAutoFast;
using System.ComponentModel.DataAnnotations;

namespace ApiAutoFast.Sample.Server.Database;

public class Blog : IEntity
{
    public Blog()
    {
    }

    public Identifier Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }
    public string Title { get; set; }
    [Required]
    public Author Author { get; set; }
    public ApiAutoFast.Identifier AuthorId { get; set; }
}
