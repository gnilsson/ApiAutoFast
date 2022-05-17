using ApiAutoFast;
using ApiAutoFast.Sample.Server.Database;

namespace ApiAutoFast.Sample.Server.Database
{
    public static partial class BlogMapper
    {
        public static BlogResponse AdaptToResponse(this Blog p1)
        {
            return p1 == null ? null : new BlogResponse()
            {
                Id = p1.Id.ToString(),
                CreatedDateTime = ((IEntity)p1).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                ModifiedDateTime = ((IEntity)p1).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                Title = p1.Title.EntityValue,
                Posts = p1.Posts.EntityValue
            };
        }
        public static BlogResponse AdaptTo(this Blog p2, BlogResponse p3)
        {
            if (p2 == null)
            {
                return null;
            }
            BlogResponse result = p3 ?? new BlogResponse();
            
            result.Id = p2.Id.ToString();
            result.CreatedDateTime = ((IEntity)p2).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.ModifiedDateTime = ((IEntity)p2).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.Title = p2.Title.EntityValue;
            result.Posts = p2.Posts.EntityValue;
            return result;
            
        }
    }
}