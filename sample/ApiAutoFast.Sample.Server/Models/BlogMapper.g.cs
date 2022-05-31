using System.Collections.Generic;
using ApiAutoFast;
using ApiAutoFast.Sample.Server;
using Mapster;
using Mapster.Utils;

namespace ApiAutoFast.Sample.Server
{
    public static partial class BlogMapper
    {
        public static BlogResponse AdaptToResponse(this Blog p1)
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
                ReferenceTuple key = new ReferenceTuple(p1, typeof(BlogResponse));
                
                if (references.TryGetValue(key, out cache))
                {
                    return (BlogResponse)cache;
                }
                BlogResponse result = new BlogResponse();
                references[key] = (object)result;
                
                result.Id = p1.Id.ToString();
                result.CreatedDateTime = ((IEntity)p1).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.ModifiedDateTime = ((IEntity)p1).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.Title = p1.Title.EntityValue;
                result.Posts = funcMain1(p1.Posts) ?? new List<PostResponse>();
                return result;
            }
            finally
            {
                scope.Dispose();
            }
            
        }
        public static BlogResponse AdaptTo(this Blog p4, BlogResponse p5)
        {
            if (p4 == null)
            {
                return null;
            }
            MapContextScope scope = new MapContextScope();
            
            try
            {
                object cache;
                
                Dictionary<ReferenceTuple, object> references = scope.Context.References;
                ReferenceTuple key = new ReferenceTuple(p4, typeof(BlogResponse));
                
                if (references.TryGetValue(key, out cache))
                {
                    return (BlogResponse)cache;
                }
                BlogResponse result = p5 ?? new BlogResponse();
                references[key] = (object)result;
                
                result.Id = p4.Id.ToString();
                result.CreatedDateTime = ((IEntity)p4).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.ModifiedDateTime = ((IEntity)p4).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.Title = p4.Title.EntityValue;
                result.Posts = funcMain3(p4.Posts, result.Posts) ?? new List<PostResponse>();
                return result;
            }
            finally
            {
                scope.Dispose();
            }
            
        }
        
        private static ICollection<PostResponse> funcMain1(ICollection<Post> p2)
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
                ReferenceTuple key = new ReferenceTuple(p2, typeof(ICollection<PostResponse>));
                
                if (references.TryGetValue(key, out cache))
                {
                    return (ICollection<PostResponse>)cache;
                }
                ICollection<PostResponse> result = new List<PostResponse>(p2.Count);
                references[key] = (object)result;
                
                IEnumerator<Post> enumerator = p2.GetEnumerator();
                
                while (enumerator.MoveNext())
                {
                    Post item = enumerator.Current;
                    result.Add(funcMain2(item));
                }
                return result;
            }
            finally
            {
                scope.Dispose();
            }
            
        }
        
        private static ICollection<PostResponse> funcMain3(ICollection<Post> p6, ICollection<PostResponse> p7)
        {
            if (p6 == null)
            {
                return null;
            }
            MapContextScope scope = new MapContextScope();
            
            try
            {
                object cache;
                
                Dictionary<ReferenceTuple, object> references = scope.Context.References;
                ReferenceTuple key = new ReferenceTuple(p6, typeof(ICollection<PostResponse>));
                
                if (references.TryGetValue(key, out cache))
                {
                    return (ICollection<PostResponse>)cache;
                }
                ICollection<PostResponse> result = new List<PostResponse>(p6.Count);
                references[key] = (object)result;
                
                IEnumerator<Post> enumerator = p6.GetEnumerator();
                
                while (enumerator.MoveNext())
                {
                    Post item = enumerator.Current;
                    result.Add(funcMain4(item));
                }
                return result;
            }
            finally
            {
                scope.Dispose();
            }
            
        }
        
        private static PostResponse funcMain2(Post p3)
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
                PostResponse result = new PostResponse();
                references[key] = (object)result;
                
                result.Id = p3.Id.ToString();
                result.CreatedDateTime = ((IEntity)p3).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.ModifiedDateTime = ((IEntity)p3).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.Title = p3.Title.EntityValue;
                result.PublicationDateTime = p3.PublicationDateTime.ToString();
                result.Description = p3.Description.EntityValue;
                result.PostType = p3.PostType.ToString();
                result.LikeCount = p3.LikeCount.EntityValue;
                result.Blog = null;
                result.BlogId = p3.BlogId.ToString();
                return result;
            }
            finally
            {
                scope.Dispose();
            }
            
        }
        
        private static PostResponse funcMain4(Post p8)
        {
            if (p8 == null)
            {
                return null;
            }
            MapContextScope scope = new MapContextScope();
            
            try
            {
                object cache;
                
                Dictionary<ReferenceTuple, object> references = scope.Context.References;
                ReferenceTuple key = new ReferenceTuple(p8, typeof(PostResponse));
                
                if (references.TryGetValue(key, out cache))
                {
                    return (PostResponse)cache;
                }
                PostResponse result = new PostResponse();
                references[key] = (object)result;
                
                result.Id = p8.Id.ToString();
                result.CreatedDateTime = ((IEntity)p8).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.ModifiedDateTime = ((IEntity)p8).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
                result.Title = p8.Title.EntityValue;
                result.PublicationDateTime = p8.PublicationDateTime.ToString();
                result.Description = p8.Description.EntityValue;
                result.PostType = p8.PostType.ToString();
                result.LikeCount = p8.LikeCount.EntityValue;
                result.Blog = null;
                result.BlogId = p8.BlogId.ToString();
                return result;
            }
            finally
            {
                scope.Dispose();
            }
            
        }
    }
}