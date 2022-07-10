using System.Collections.Generic;
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
                result.CreatedDateTime = p1.CreatedDateTime.ToString();
                result.ModifiedDateTime = p1.ModifiedDateTime.ToString();
                result.Title = p1.Title.EntityValue;
                return result;
            }
            finally
            {
                scope.Dispose();
            }
            
        }
        public static BlogResponse AdaptTo(this Blog p2, BlogResponse p3)
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
                BlogResponse result = p3 ?? new BlogResponse();
                references[key] = (object)result;
                
                result.Id = p2.Id.ToString();
                result.CreatedDateTime = p2.CreatedDateTime.ToString();
                result.ModifiedDateTime = p2.ModifiedDateTime.ToString();
                result.Title = p2.Title.EntityValue;
                return result;
            }
            finally
            {
                scope.Dispose();
            }
            
        }
    }
}