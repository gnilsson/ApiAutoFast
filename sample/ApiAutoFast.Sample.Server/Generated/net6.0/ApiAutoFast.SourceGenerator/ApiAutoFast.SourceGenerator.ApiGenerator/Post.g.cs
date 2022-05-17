﻿
using ApiAutoFast;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public class Post : IEntity
{
    public Post()
    {
    }

    public Identifier Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }
    public string Title { get; set; }
    public System.DateTime PublicationDateTime { get; set; }
    public string Description { get; set; }
    public ApiAutoFast.Sample.Server.Database.EPostType PostType { get; set; }
    public int LikeCount { get; set; }
    public Identifier BlogId { get; set; }
    public Blog Blog { get; set; }
}
