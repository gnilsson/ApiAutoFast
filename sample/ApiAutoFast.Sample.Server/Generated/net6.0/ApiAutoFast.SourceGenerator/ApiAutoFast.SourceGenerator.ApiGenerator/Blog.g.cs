﻿
using ApiAutoFast;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ApiAutoFast.Sample.Server;

public class Blog : IEntity<SequentialIdentifier>
{
    public Blog()
    {
    }

    public SequentialIdentifier Id { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public DateTime ModifiedDateTime { get; set; }
    public Title Title { get; set; }
}
