//using ApiAutoFast.Domain;
//using ApiAutoFast.Enums;
//using GN.Toolkit;
//using System.ComponentModel.DataAnnotations;

//namespace ApiAutoFast;

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
