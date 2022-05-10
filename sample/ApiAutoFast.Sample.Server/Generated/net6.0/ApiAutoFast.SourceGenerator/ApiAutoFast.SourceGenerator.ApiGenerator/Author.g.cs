
using ApiAutoFast;
using System.ComponentModel.DataAnnotations;

namespace ApiAutoFast.Sample.Server.Database;

public class Author : IEntity
{

    public Author()
    {
        this.Blogs = new HashSet<Blog>();

    }


    public Identifier Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }
    [Required]
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public ApiAutoFast.Sample.Server.Database.ProfessionCategory? Profession { get; set; }
    public ICollection<Blog> Blogs { get; set; }
}
