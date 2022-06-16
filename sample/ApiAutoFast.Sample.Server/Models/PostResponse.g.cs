using ApiAutoFast.Sample.Server;

namespace ApiAutoFast.Sample.Server
{
    public partial class PostResponse
    {
        public string Id { get; set; }
        public string CreatedDateTime { get; set; }
        public string ModifiedDateTime { get; set; }
        public string Title { get; set; }
        public string PublicationDateTime { get; set; }
        public string Description { get; set; }
        public string PostType { get; set; }
        public int LikeCount { get; set; }
        public BlogResponse Blog { get; set; }
        public string BlogId { get; set; }
    }
}