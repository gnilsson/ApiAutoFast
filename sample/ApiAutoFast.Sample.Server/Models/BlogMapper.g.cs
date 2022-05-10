using System.Collections.Generic;
using ApiAutoFast;
using ApiAutoFast.Sample.Server.Database;
using Mapster;

namespace ApiAutoFast.Sample.Server.Database
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
                Title = p1.Title,
                Author = funcMain1(p1.Author),
                AuthorId = p1.AuthorId
            };
        }
        public static BlogResponse AdaptTo(this Blog p4, BlogResponse p5)
        {
            if (p4 == null)
            {
                return null;
            }
            BlogResponse result = p5 ?? new BlogResponse();
            
            result.Id = p4.Id.ToString();
            result.CreatedDateTime = ((IEntity)p4).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.ModifiedDateTime = ((IEntity)p4).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.Title = p4.Title;
            result.Author = funcMain3(p4.Author, result.Author);
            result.AuthorId = p4.AuthorId;
            return result;
            
        }
        
        private static AuthorResponse funcMain1(Author p2)
        {
            return p2 == null ? null : new AuthorResponse()
            {
                Id = p2.Id.ToString(),
                CreatedDateTime = ((IEntity)p2).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                ModifiedDateTime = ((IEntity)p2).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                FirstName = p2.FirstName,
                LastName = p2.LastName,
                Profession = p2.Profession,
                Blogs = funcMain2(p2.Blogs)
            };
        }
        
        private static AuthorResponse funcMain3(Author p6, AuthorResponse p7)
        {
            if (p6 == null)
            {
                return null;
            }
            AuthorResponse result = p7 ?? new AuthorResponse();
            
            result.Id = p6.Id.ToString();
            result.CreatedDateTime = ((IEntity)p6).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.ModifiedDateTime = ((IEntity)p6).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.FirstName = p6.FirstName;
            result.LastName = p6.LastName;
            result.Profession = p6.Profession;
            result.Blogs = funcMain4(p6.Blogs, result.Blogs);
            return result;
            
        }
        
        private static ICollection<BlogResponse> funcMain2(ICollection<Blog> p3)
        {
            if (p3 == null)
            {
                return null;
            }
            ICollection<BlogResponse> result = new List<BlogResponse>(p3.Count);
            
            IEnumerator<Blog> enumerator = p3.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Blog item = enumerator.Current;
                result.Add(TypeAdapterConfig1.GetMapFunction<Blog, BlogResponse>().Invoke(item));
            }
            return result;
            
        }
        
        private static ICollection<BlogResponse> funcMain4(ICollection<Blog> p8, ICollection<BlogResponse> p9)
        {
            if (p8 == null)
            {
                return null;
            }
            ICollection<BlogResponse> result = new List<BlogResponse>(p8.Count);
            
            IEnumerator<Blog> enumerator = p8.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Blog item = enumerator.Current;
                result.Add(TypeAdapterConfig1.GetMapFunction<Blog, BlogResponse>().Invoke(item));
            }
            return result;
            
        }
    }
}