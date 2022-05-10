using ApiAutoFast;
using ApiAutoFast.Sample.Server.Database;

namespace ApiAutoFast.Sample.Server.Database
{
    public partial class BlogResponse
    {
        public string Id { get; set; }
        public string CreatedDateTime { get; set; }
        public string ModifiedDateTime { get; set; }
        public string Title { get; set; }
        public AuthorResponse Author { get; set; }
        public Identifier AuthorId { get; set; }
    }
}