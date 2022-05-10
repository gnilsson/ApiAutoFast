using System.Collections.Generic;
using ApiAutoFast.Sample.Server.Database;

namespace ApiAutoFast.Sample.Server.Database
{
    public partial class AuthorResponse
    {
        public string? Id { get; set; }
        public string? CreatedDateTime { get; set; }
        public string? ModifiedDateTime { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public ProfessionCategory? Profession { get; set; }
        public ICollection<BlogResponse> Blogs { get; set; }
    }
}