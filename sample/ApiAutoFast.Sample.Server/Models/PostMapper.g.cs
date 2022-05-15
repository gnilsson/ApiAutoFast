using ApiAutoFast;
using ApiAutoFast.Sample.Server.Database;

namespace ApiAutoFast.Sample.Server.Database
{
    public static partial class PostMapper
    {
        public static PostResponse AdaptToResponse(this Post p1)
        {
            return p1 == null ? null : new PostResponse()
            {
                Id = p1.Id.ToString(),
                CreatedDateTime = ((IEntity)p1).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                ModifiedDateTime = ((IEntity)p1).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                Title = p1.Title == null ? null : p1.Title.ToString(),
                PublicationDateTime = p1.PublicationDateTime == null ? null : p1.PublicationDateTime.ToString(),
                Description = p1.Description == null ? null : p1.Description.ToString(),
                PostType = p1.PostType == null ? null : p1.PostType.ToString()
            };
        }
        public static PostResponse AdaptTo(this Post p2, PostResponse p3)
        {
            if (p2 == null)
            {
                return null;
            }
            PostResponse result = p3 ?? new PostResponse();
            
            result.Id = p2.Id.ToString();
            result.CreatedDateTime = ((IEntity)p2).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.ModifiedDateTime = ((IEntity)p2).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.Title = p2.Title == null ? null : p2.Title.ToString();
            result.PublicationDateTime = p2.PublicationDateTime == null ? null : p2.PublicationDateTime.ToString();
            result.Description = p2.Description == null ? null : p2.Description.ToString();
            result.PostType = p2.PostType == null ? null : p2.PostType.ToString();
            return result;
            
        }
    }
}