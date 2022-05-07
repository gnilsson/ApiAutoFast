using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ApiAutoFast.Sample.Server.Database;



//[AutoFastEndpoints]
//public class Author
//{
//    internal class Properties
//    {
//        [Required, CreateCommand]
//        public string? FirstName { get; set; }
//        [CreateCommand, QueryRequest]
//        public string? LastName { get; set; }
//        [QueryRequest]
//        public ProfessionCategory Profession { get; set; }
//    }


//    internal class Command { };

//    public partial class Request { };

//    public partial class MappingProfile
//    {
//        // override ToEntity
//    }
//}


//[JsonSerializable(typeof(Query.All.Blog))]
//[JsonSerializable(typeof(Post.Blog))]

//[JsonSerializable(typeof(BlogResponse))]
//public partial class BlogSerializerCtx : JsonSerializerContext { }


//[JsonSerializable(typeof(AuthorResponse))]
//public partial class AuthorSerializerCtx : JsonSerializerContext { }

// HHMM??? help
// thers some issues generating json attributes...
// it works when its done in postinit stage but not during compilation.
// but in postinit stage we dont have access to values.

//[AutoFast]
public class Entityx
{
    public class Author : IEntity
    {
        public Author()
        {
            this.Blogs = new HashSet<Blog>();
        }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public ProfessionCategory Profession { get; set; }
        public ICollection<Blog> Blogs { get; set; }
        public Identifier Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }

    public class Post : IEntity
    {
        public Post() { }

        public string? Title { get; set; }
        public string? Content { get; set; }

        [Required]
        public Blog Blog { get; set; } = default!;
        public Identifier BlogId { get; set; }
        public Identifier Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }

    public class Blog : IEntity
    {
        public Blog()
        {
            this.Posts = new HashSet<Post>();
            this.Authors = new HashSet<Author>();
        }

        public string? Title { get; set; }
        public BlogCategory BlogCategory { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Author> Authors { get; set; }
        public Identifier Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
    }

}

