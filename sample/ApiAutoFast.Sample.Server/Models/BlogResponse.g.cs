using System.Collections.Generic;
using ApiAutoFast.Sample.Server.Database;

namespace ApiAutoFast.Sample.Server.Database
{
    public partial class BlogResponse
    {
        public string Id { get; set; }
        public string CreatedDateTime { get; set; }
        public string ModifiedDateTime { get; set; }
        public string Title { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}