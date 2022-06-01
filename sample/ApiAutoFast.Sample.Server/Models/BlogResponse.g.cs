using System.Collections.Generic;
using ApiAutoFast.Sample.Server;

namespace ApiAutoFast.Sample.Server
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