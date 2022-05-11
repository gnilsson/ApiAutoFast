using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace ApiAutoFast.Sample.Server.Database;

// note: the next version of aaf will define properties with complex types instead of primitves, with help from a ValueOf pattern.
//       this can help with defining types that have different types across request - entity - response, aswell as aid in mapping.
//       in the best of worlds it can also be used to define validation.

[AutoFastContext]
public partial class AutoFastSampleDbContext : DbContext
{
}

[AutoFastEndpoints]
public class AuthorConfig
{
    internal class Properties
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public ProfessionCategory? Profession { get; set; }
        [ExcludeRequestModel]
        public ICollection<BlogConfig>? Blogs { get; set; }
    }
}

[AutoFastEndpoints]
public class BlogConfig
{
    internal class Properties
    {
        public string? Title { get; set; }
        public AuthorConfig? Author { get; set; }
        public Identifier AuthorId { get; set; }
    }
}


//public partial class BlogMappingProfile
//{
//    public override Blog ToEntity(BlogCreateCommand r)
//    {
//        return new Blog
//        {
//            AuthorId = (Identifier)r.AuthorId!,
//            Title = r.Title,
//        };
//    }
//}
