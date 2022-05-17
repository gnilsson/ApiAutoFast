using System;
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
                Title = p1.Title.EntityValue,
                Posts = funcMain1(p1.Posts)
            };
        }
        public static BlogResponse AdaptTo(this Blog p7, BlogResponse p8)
        {
            if (p7 == null)
            {
                return null;
            }
            BlogResponse result = p8 ?? new BlogResponse();

            result.Id = p7.Id.ToString();
            result.CreatedDateTime = ((IEntity)p7).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.ModifiedDateTime = ((IEntity)p7).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm");
            result.Title = p7.Title.EntityValue;
            result.Posts = funcMain6(p7.Posts, result.Posts);
            return result;

        }

        private static ICollection<Post> funcMain1(ICollection<Post> p2)
        {
            if (p2 == null)
            {
                return null;
            }
            ICollection<Post> result = new List<Post>(p2.Count);

            IEnumerator<Post> enumerator = p2.GetEnumerator();

            while (enumerator.MoveNext())
            {
                Post item = enumerator.Current;
                result.Add(item == null ? null : new Post()
                {
                    Id = item.Id,
                    CreatedDateTime = funcMain2(((IEntity)item).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm")),
                    ModifiedDateTime = funcMain3(((IEntity)item).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm")),
                    Title = item.Title,
                    PublicationDateTime = item.PublicationDateTime,
                    Description = item.Description,
                    PostType = item.PostType,
                    LikeCount = item.LikeCount,
                    BlogId = item.BlogId,
                    Blog = item.Blog == null ? null : new Blog()
                    {
                        Id = item.Blog.Id,
                        CreatedDateTime = funcMain4(((IEntity)item.Blog).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm")),
                        ModifiedDateTime = funcMain5(((IEntity)item.Blog).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm")),
                        Title = item.Blog.Title,
                        Posts = TypeAdapterConfig1.GetMapFunction<ICollection<Post>, ICollection<Post>>().Invoke(item.Blog.Posts)
                    }
                });
            }
            return result;

        }

        private static ICollection<Post> funcMain6(ICollection<Post> p9, ICollection<Post> p10)
        {
            if (p9 == null)
            {
                return null;
            }
            ICollection<Post> result = new List<Post>(p9.Count);

            IEnumerator<Post> enumerator = p9.GetEnumerator();

            while (enumerator.MoveNext())
            {
                Post item = enumerator.Current;
                result.Add(item == null ? null : new Post()
                {
                    Id = item.Id,
                    CreatedDateTime = funcMain7(((IEntity)item).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm")),
                    ModifiedDateTime = funcMain8(((IEntity)item).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm")),
                    Title = item.Title,
                    PublicationDateTime = item.PublicationDateTime,
                    Description = item.Description,
                    PostType = item.PostType,
                    LikeCount = item.LikeCount,
                    BlogId = item.BlogId,
                    Blog = item.Blog == null ? null : new Blog()
                    {
                        Id = item.Blog.Id,
                        CreatedDateTime = funcMain9(((IEntity)item.Blog).CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm")),
                        ModifiedDateTime = funcMain10(((IEntity)item.Blog).ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm")),
                        Title = item.Blog.Title,
                        Posts = TypeAdapterConfig1.GetMapFunction<ICollection<Post>, ICollection<Post>>().Invoke(item.Blog.Posts)
                    }
                });
            }
            return result;

        }

        private static DateTime funcMain2(string p3)
        {
            return p3 == null ? default(DateTime) : DateTime.Parse(p3);
        }

        private static DateTime funcMain3(string p4)
        {
            return p4 == null ? default(DateTime) : DateTime.Parse(p4);
        }

        private static DateTime funcMain4(string p5)
        {
            return p5 == null ? default(DateTime) : DateTime.Parse(p5);
        }

        private static DateTime funcMain5(string p6)
        {
            return p6 == null ? default(DateTime) : DateTime.Parse(p6);
        }

        private static DateTime funcMain7(string p11)
        {
            return p11 == null ? default(DateTime) : DateTime.Parse(p11);
        }

        private static DateTime funcMain8(string p12)
        {
            return p12 == null ? default(DateTime) : DateTime.Parse(p12);
        }

        private static DateTime funcMain9(string p13)
        {
            return p13 == null ? default(DateTime) : DateTime.Parse(p13);
        }

        private static DateTime funcMain10(string p14)
        {
            return p14 == null ? default(DateTime) : DateTime.Parse(p14);
        }
    }
}