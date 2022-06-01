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
                result.Blog = p1.Blog == null ? null : p1.Blog.ToString();
                result.BlogId = p1.BlogId.ToString();
                return result;
            }
            finally
            {
                scope.Dispose();
            }
            
        }
        public static PostResponse AdaptTo(this Post p2, PostResponse p3)
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
                ReferenceTuple key = new ReferenceTuple(p2, typeof(PostResponse));
                
                if (references.TryGetValue(key, out cache))
                {
                    return (PostResponse)cache;
                }
                PostResponse result = p3 ?? new PostResponse();
                references[key] = (object)result;
                
                result.Id = p2.Id.ToString();
                result.CreatedDateTime = ((IEntity)p2).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.ModifiedDateTime = ((IEntity)p2).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.Title = p2.Title.EntityValue;
                result.PublicationDateTime = p2.PublicationDateTime.ToString();
                result.Description = p2.Description.EntityValue;
                result.PostType = p2.PostType.ToString();
                result.LikeCount = p2.LikeCount.EntityValue;
                result.Blog = p2.Blog == null ? null : p2.Blog.ToString();
                result.BlogId = p2.BlogId.ToString();
                return result;
            }
            finally
            {
                scope.Dispose();
            }
            
        }
    }
}