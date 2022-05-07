//using System.Threading.Tasks;
//using VerifyXunit;
//using Xunit;

//namespace ApiAutoFast.Tests;


//[UsesVerify]
//public class SnapshotTests
//{
//    [Fact]
//    public Task GeneratesEntityDbPropertiesCorrectly()
//    {
//        var source = @"
//using ApiAutoFast.Domain;
//using ApiAutoFast.Sample.Server.Database.Enums;
//using GN.Toolkit;
//using System.ComponentModel.DataAnnotations;


//namespace ApiAutoFast.Sample.Server.Database.Enums;

//public enum ProfessionCategory
//{
//    None = 0,
//    Unemployed,
//    Programmer,
//    CoalmineWorker,
//    Botanist,
//    SpacestationArchitect,
//    Dragon
//}

//namespace ApiAutoFast.Sample.Server.Database;

//[AutoFastContext]
//public class AutoFastSampleDbContext : AutoFastDbContext
//{
//    public AutoFastSampleDbContext(DbContextOptions<AutoFastDbContext> options) : base(options)
//    { }
//}

//[AutoFast]
//public class Entity
//{
//    public class Author : IEntity
//    {
//        public Author()
//        {
//            this.Blogs = new HashSet<Blog>();
//        }

//        public string? FirstName { get; set; }
//        public string? LastName { get; set; }
//        public ProfessionCategory Profession { get; set; }
//        public ICollection<Blog> Blogs { get; set; }
//        public Identifier Id { get; set; }
//        public DateTime CreatedDateTime { get; set; }
//        public DateTime ModifiedDateTime { get; set; }
//    }

//    public class Post : IEntity
//    {
//        public Post() { }

//        public string? Title { get; set; }
//        public string? Content { get; set; }

//        [Required]
//        public Blog Blog { get; set; } = default!;
//        public Identifier BlogId { get; set; }
//        public Identifier Id { get; set; }
//        public DateTime CreatedDateTime { get; set; }
//        public DateTime ModifiedDateTime { get; set; }
//    }

//    public class Blog : IEntity
//    {
//        public Blog()
//        {
//            this.Posts = new HashSet<Post>();
//            this.Authors = new HashSet<Author>();
//        }

//        public string? Title { get; set; }
//        public BlogCategory BlogCategory { get; set; }
//        public ICollection<Post> Posts { get; set; }
//        public ICollection<Author> Authors { get; set; }
//        public Identifier Id { get; set; }
//        public DateTime CreatedDateTime { get; set; }
//        public DateTime ModifiedDateTime { get; set; }
//    }
//}

//";

//        //var source = @"using ApiAutoFast;

//        //[AutoFast]
//        //public class Entity
//        //{
//        //    public class Blog : IEntity
//        //    {
//        //        public Blog()
//        //        {
//        //            this.Posts = new HashSet<Post>();
//        //            this.Authors = new HashSet<Author>();
//        //        }

//        //        public string Title { get; set; } = default!;
//        //        public BlogCategory BlogCategory { get; set; }
//        //        public ICollection<Post> Posts { get; set; }
//        //        public ICollection<Author> Authors { get; set; }
//        //        public Identifier Id { get; set; }
//        //        public DateTime CreatedDate { get; set; }
//        //        public DateTime UpdatedDate { get; set; }
//        //    }

//        //    public class Author : IEntity
//        //    {
//        //        public Author()
//        //        {
//        //            this.Blogs = new HashSet<Blog>();
//        //        }

//        //        public string FirstName { get; set; } = default!;
//        //        public string LastName { get; set; } = default!;
//        //        public ProfessionCategory Profession { get; set; }
//        //        public ICollection<Blog> Blogs { get; set; }
//        //        public Identifier Id { get; set; }
//        //        public DateTime CreatedDate { get; set; }
//        //        public DateTime UpdatedDate { get; set; }
//        //    }

//        //    public class Post : IEntity
//        //    {
//        //        public Post() { }

//        //        public string Title { get; set; } = default!;
//        //        public string Content { get; set; } = default!;
//        //        [Required]
//        //        public Blog Blog { get; set; } = default!;
//        //        public Identifier BlogId { get; set; } = default!;
//        //        public Identifier Id { get; set; }
//        //        public DateTime CreatedDate { get; set; }
//        //        public DateTime UpdatedDate { get; set; }
//        //    }
//        //}";

//        return TestHelper.Verify(source);
//    }
//}
