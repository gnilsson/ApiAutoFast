using System.Collections.Generic;
using ApiAutoFast;
using ApiAutoFast.Sample.Server.Database;
using Mapster;
using Mapster.Utils;

namespace ApiAutoFast.Sample.Server.Database
{
    public static partial class AuthorMapper
    {
        private static TypeAdapterConfig TypeAdapterConfig1;
        
        public static AuthorResponse AdaptToResponse(this Author p1)
        {
            return p1 == null ? null : new AuthorResponse()
            {
                Id = p1.Id.ToString(),
                CreatedDateTime = ((IEntity)p1).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                ModifiedDateTime = ((IEntity)p1).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                FirstName = p1.FirstName,
                LastName = p1.LastName,
                Profession = p1.Profession == null ? null : Enum<ProfessionCategory>.ToString((ProfessionCategory)p1.Profession),
                Blogs = funcMain1(p1.Blogs)
            };
        }
        public static AuthorResponse AdaptTo(this Author p3, AuthorResponse p4)
        {
            if (p3 == null)
            {
                return null;
            }
            AuthorResponse result = p4 ?? new AuthorResponse();
            
            result.Id = p3.Id.ToString();
            result.CreatedDateTime = ((IEntity)p3).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.ModifiedDateTime = ((IEntity)p3).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.FirstName = p3.FirstName;
            result.LastName = p3.LastName;
            result.Profession = p3.Profession == null ? null : Enum<ProfessionCategory>.ToString((ProfessionCategory)p3.Profession);
            result.Blogs = funcMain2(p3.Blogs, result.Blogs);
            return result;
            
        }
        
        private static ICollection<BlogResponse> funcMain1(ICollection<Blog> p2)
        {
            if (p2 == null)
            {
                return null;
            }
            ICollection<BlogResponse> result = new List<BlogResponse>(p2.Count);
            
            IEnumerator<Blog> enumerator = p2.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Blog item = enumerator.Current;
                result.Add(item == null ? null : new BlogResponse()
                {
                    Id = item.Id.ToString(),
                    CreatedDateTime = ((IEntity)item).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                    ModifiedDateTime = ((IEntity)item).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                    Title = item.Title,
                    Author = TypeAdapterConfig1.GetMapFunction<Author, AuthorResponse>().Invoke(item.Author),
                    AuthorId = item.AuthorId.ToString()
                });
            }
            return result;
            
        }
        
        private static ICollection<BlogResponse> funcMain2(ICollection<Blog> p5, ICollection<BlogResponse> p6)
        {
            if (p5 == null)
            {
                return null;
            }
            ICollection<BlogResponse> result = new List<BlogResponse>(p5.Count);
            
            IEnumerator<Blog> enumerator = p5.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Blog item = enumerator.Current;
                result.Add(item == null ? null : new BlogResponse()
                {
                    Id = item.Id.ToString(),
                    CreatedDateTime = ((IEntity)item).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                    ModifiedDateTime = ((IEntity)item).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"),
                    Title = item.Title,
                    Author = TypeAdapterConfig1.GetMapFunction<Author, AuthorResponse>().Invoke(item.Author),
                    AuthorId = item.AuthorId.ToString()
                });
            }
            return result;
            
        }
    }
}