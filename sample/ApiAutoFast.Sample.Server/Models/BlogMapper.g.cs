using System.Collections.Generic;
using ApiAutoFast;
using ApiAutoFast.Sample.Server;
using Mapster;

namespace ApiAutoFast.Sample.Server
{
    public static partial class BlogMapper
    {
        private static TypeAdapterConfig TypeAdapterConfig1;
        
        public static BlogResponse AdaptToResponse(this Blog p1)
        {
            return p1 == null ? null : new BlogResponse()
            {
                Id = p1.Id.ToString(),
                CreatedDateTime = ((IEntity)p1).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                ModifiedDateTime = ((IEntity)p1).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                Title = p1.Title.EntityValue,
                Posts = funcMain1(p1.Posts) ?? new List<PostResponse>()
            };
        }
        public static BlogResponse AdaptTo(this Blog p3, BlogResponse p4)
        {
            if (p3 == null)
            {
                return null;
            }
            BlogResponse result = p4 ?? new BlogResponse();
            
            result.Id = p3.Id.ToString();
            result.CreatedDateTime = ((IEntity)p3).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.ModifiedDateTime = ((IEntity)p3).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.Title = p3.Title.EntityValue;
            result.Posts = funcMain2(p3.Posts, result.Posts) ?? new List<PostResponse>();
            return result;
            
        }
        
        private static ICollection<PostResponse> funcMain1(ICollection<Post> p2)
        {
            if (p2 == null)
            {
                return null;
            }
            ICollection<PostResponse> result = new List<PostResponse>(p2.Count);
            
            IEnumerator<Post> enumerator = p2.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Post item = enumerator.Current;
                result.Add(item == null ? null : new PostResponse()
                {
                    Id = item.Id.ToString(),
                    CreatedDateTime = ((IEntity)item).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                    ModifiedDateTime = ((IEntity)item).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                    Title = item.Title.EntityValue,
                    PublicationDateTime = item.PublicationDateTime.ToString(),
                    Description = item.Description.EntityValue,
                    PostType = item.PostType.ToString(),
                    LikeCount = item.LikeCount.EntityValue,
                    BlogId = item.BlogId.ToString(),
                    Blog = TypeAdapterConfig1.GetMapFunction<Blog, BlogResponse>().Invoke(item.Blog)
                });
            }
            return result;
            
        }
        
        private static ICollection<PostResponse> funcMain2(ICollection<Post> p5, ICollection<PostResponse> p6)
        {
            if (p5 == null)
            {
                return null;
            }
            ICollection<PostResponse> result = new List<PostResponse>(p5.Count);
            
            IEnumerator<Post> enumerator = p5.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Post item = enumerator.Current;
                result.Add(item == null ? null : new PostResponse()
                {
                    Id = item.Id.ToString(),
                    CreatedDateTime = ((IEntity)item).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                    ModifiedDateTime = ((IEntity)item).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                    Title = item.Title.EntityValue,
                    PublicationDateTime = item.PublicationDateTime.ToString(),
                    Description = item.Description.EntityValue,
                    PostType = item.PostType.ToString(),
                    LikeCount = item.LikeCount.EntityValue,
                    BlogId = item.BlogId.ToString(),
                    Blog = TypeAdapterConfig1.GetMapFunction<Blog, BlogResponse>().Invoke(item.Blog)
                });
            }
            return result;
            
        }
    }
}