using System.Collections.Generic;
using ApiAutoFast;
using ApiAutoFast.Sample.Server;
using Mapster;
using Mapster.Utils;

namespace ApiAutoFast.Sample.Server
{
    public static partial class PostMapper
    {
        public static PostResponse AdaptToResponse(this Post p1)
        {
            if (p1 == null)
            {
                return null;
            }
            MapContextScope scope = new MapContextScope();
            
            try
            {
                object cache;
                
                Dictionary<ReferenceTuple, object> references = scope.Context.References;
                ReferenceTuple key = new ReferenceTuple(p1, typeof(PostResponse));
                
                if (references.TryGetValue(key, out cache))
                {
                    return (PostResponse)cache;
                }
                PostResponse result = new PostResponse();
                references[key] = (object)result;
                
                result.Id = p1.Id.ToString();
                result.CreatedDateTime = ((IEntity)p1).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.ModifiedDateTime = ((IEntity)p1).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.Title = p1.Title.EntityValue;
                result.PublicationDateTime = p1.PublicationDateTime.ToString();
                result.Description = p1.Description.EntityValue;
                result.PostType = p1.PostType.ToString();
                result.LikeCount = p1.LikeCount.EntityValue;
                result.Blog = funcMain1(p1.Blog);
                result.BlogId = p1.BlogId.ToString();
                return result;
            }
            finally
            {
                scope.Dispose();
            }
            
        }
        public static PostResponse AdaptTo(this Post p3, PostResponse p4)
        {
            if (p3 == null)
            {
                return null;
            }
            MapContextScope scope = new MapContextScope();
            
            try
            {
                object cache;
                
                Dictionary<ReferenceTuple, object> references = scope.Context.References;
                ReferenceTuple key = new ReferenceTuple(p3, typeof(PostResponse));
                
                if (references.TryGetValue(key, out cache))
                {
                    return (PostResponse)cache;
                }
                PostResponse result = p4 ?? new PostResponse();
                references[key] = (object)result;
                
                result.Id = p3.Id.ToString();
                result.CreatedDateTime = ((IEntity)p3).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.ModifiedDateTime = ((IEntity)p3).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.Title = p3.Title.EntityValue;
                result.PublicationDateTime = p3.PublicationDateTime.ToString();
                result.Description = p3.Description.EntityValue;
                result.PostType = p3.PostType.ToString();
                result.LikeCount = p3.LikeCount.EntityValue;
                result.Blog = funcMain2(p3.Blog, result.Blog);
                result.BlogId = p3.BlogId.ToString();
                return result;
            }
            finally
            {
                scope.Dispose();
            }
            
        }
        
        private static BlogResponse funcMain1(Blog p2)
        {
            if (p2 == null)
            {
                return null;
            }
            MapContextScope scope = new MapContextScope();
            
            try
            {
                object cache;
                
                Dictionary<ReferenceTuple, object> references = scope.Context.References;
                ReferenceTuple key = new ReferenceTuple(p2, typeof(BlogResponse));
                
                if (references.TryGetValue(key, out cache))
                {
                    return (BlogResponse)cache;
                }
                BlogResponse result = new BlogResponse();
                references[key] = (object)result;
                
                result.Id = p2.Id.ToString();
                result.CreatedDateTime = ((IEntity)p2).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.ModifiedDateTime = ((IEntity)p2).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.Title = p2.Title.EntityValue;
                result.Posts = null ?? new List<PostResponse>();
                return result;
            }
            finally
            {
                scope.Dispose();
            }
            
        }
        
        private static BlogResponse funcMain2(Blog p5, BlogResponse p6)
        {
            if (p5 == null)
            {
                return null;
            }
            MapContextScope scope = new MapContextScope();
            
            try
            {
                object cache;
                
                Dictionary<ReferenceTuple, object> references = scope.Context.References;
                ReferenceTuple key = new ReferenceTuple(p5, typeof(BlogResponse));
                
                if (references.TryGetValue(key, out cache))
                {
                    return (BlogResponse)cache;
                }
                BlogResponse result = p6 ?? new BlogResponse();
                references[key] = (object)result;
                
                result.Id = p5.Id.ToString();
                result.CreatedDateTime = ((IEntity)p5).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.ModifiedDateTime = ((IEntity)p5).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.Title = p5.Title.EntityValue;
                result.Posts = null ?? new List<PostResponse>();
                return result;
            }
            finally
            {
                scope.Dispose();
            }
            
        }
    }
}