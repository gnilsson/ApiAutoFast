using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace ApiAutoFast.Sample.Server.Database;

//[AutoFastContext]
public partial class AutoFastSampleDbContext : DbContext
{
}


//[AutoFastEndpoints("Author")]
public class AuthorConfig
{
    internal class Properties
    {
        [Required, CreateCommand]
        public string? FirstName { get; set; }
        [CreateCommand, QueryRequest]
        public string? LastName { get; set; }
        [QueryRequest, CreateCommand, ModifyCommand]
        public ProfessionCategory? Profession { get; set; }
        //public ICollection<BlogConfig>? Blogs { get; set; }
    }
}

//public class BlogConfig
//{
//    internal class Properties
//    {
//        [CreateCommand, ModifyCommand, QueryRequest]
//        public string Title { get; set; } = default!;
//        [Required]
//        public AuthorConfig Author { get; set; } = default!;
//        [CreateCommand, QueryRequest]
//        public Identifier AuthorId { get; set; }
//    }
//}
