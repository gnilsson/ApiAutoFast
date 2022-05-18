using System.Collections.Generic;
using ApiAutoFast;
using ApiAutoFast.Sample.Server.Database;
using Mapster;

namespace ApiAutoFast.Sample.Server.Database
{
    public static partial class PostMapper
    {
        private static TypeAdapterConfig TypeAdapterConfig1;
        
        public static PostResponse AdaptToResponse(this Post p1)
        {
            return p1 == null ? null : new PostResponse()
            {
                Id = p1.Id.ToString(),
                CreatedDateTime = ((IEntity)p1).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                ModifiedDateTime = ((IEntity)p1).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                Title = p1.Title.EntityValue,
                PublicationDateTime = p1.PublicationDateTime.ToString(),
                Description = p1.Description.EntityValue,
                PostType = p1.PostType.ToString(),
                LikeCount = p1.LikeCount.EntityValue,
                BlogId = p1.BlogId.ToString(),
                Blog = funcMain1(p1.Blog)
            };
        }
        public static PostResponse AdaptTo(this Post p4, PostResponse p5)
        {
            if (p4 == null)
            {
                return null;
            }
            PostResponse result = p5 ?? new PostResponse();
            
            result.Id = p4.Id.ToString();
            result.CreatedDateTime = ((IEntity)p4).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.ModifiedDateTime = ((IEntity)p4).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.Title = p4.Title.EntityValue;
            result.PublicationDateTime = p4.PublicationDateTime.ToString();
            result.Description = p4.Description.EntityValue;
            result.PostType = p4.PostType.ToString();
            result.LikeCount = p4.LikeCount.EntityValue;
            result.BlogId = p4.BlogId.ToString();
            result.Blog = funcMain3(p4.Blog, result.Blog);
            return result;
            
        }
        
        private static BlogResponse funcMain1(Blog p2)
        {
            return p2 == null ? null : new BlogResponse()
            {
                Id = p2.Id.ToString(),
                CreatedDateTime = ((IEntity)p2).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                ModifiedDateTime = ((IEntity)p2).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                Title = p2.Title.EntityValue,
                Posts = funcMain2(p2.Posts) ?? new List<PostResponse>()
            };
        }
        
        private static BlogResponse funcMain3(Blog p6, BlogResponse p7)
        {
            if (p6 == null)
            {
                return null;
            }
            BlogResponse result = p7 ?? new BlogResponse();
            
            result.Id = p6.Id.ToString();
            result.CreatedDateTime = ((IEntity)p6).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.ModifiedDateTime = ((IEntity)p6).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.Title = p6.Title.EntityValue;
            result.Posts = funcMain4(p6.Posts, result.Posts) ?? new List<PostResponse>();
            return result;
            
        }
        
        private static ICollection<PostResponse> funcMain2(ICollection<Post> p3)
        {
            if (p3 == null)
            {
                return null;
            }
            ICollection<PostResponse> result = new List<PostResponse>(p3.Count);
            
            IEnumerator<Post> enumerator = p3.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Post item = enumerator.Current;
                result.Add(TypeAdapterConfig1.GetMapFunction<Post, PostResponse>().Invoke(item));
            }
            return result;
            
        }
        
        private static ICollection<PostResponse> funcMain4(ICollection<Post> p8, ICollection<PostResponse> p9)
        {
            if (p8 == null)
            {
                return null;
            }
            ICollection<PostResponse> result = new List<PostResponse>(p8.Count);
            
            IEnumerator<Post> enumerator = p8.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Post item = enumerator.Current;
                result.Add(TypeAdapterConfig1.GetMapFunction<Post, PostResponse>().Invoke(item));
            }
            return result;
            
        }
    }
}