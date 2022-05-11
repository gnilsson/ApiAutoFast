﻿
using ApiAutoFast;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server.Database;

public class Author : IEntity
{
    public Author()
    {
        this.Blogs = new HashSet<Blog>();
    }

    public Identifier Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [Required]
    public ApiAutoFast.Sample.Server.Database.ProfessionCategory? Profession { get; set; }
    public ICollection<Blog> Blogs { get; set; }
}
