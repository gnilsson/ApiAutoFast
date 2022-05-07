using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ApiAutoFast;
using Mapster;

namespace ApiAutoFast.Sample.Server.Database;

[AutoFastContext]
public partial class AutoFastSampleDbContext : DbContext
{
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}


[AutoFastEndpoints]
public class AuthorConfig
{
    internal class Properties
    {
        [Required, CreateCommand]
        public string? FirstName { get; set; }
        [CreateCommand, QueryRequest]
        public string? LastName { get; set; }
        [QueryRequest, CreateCommand, ModifyCommand]
        public ProfessionCategory? Profession { get; set; }
    }
}

public partial class MappingRegister
{
    public MappingRegister()
    {
        _extendRegisterResponses = true;
    }

    static partial void OnExtendRegisterResponses(AdaptAttributeBuilder aab)
    {
        aab.ForType<Author>(cfg =>
        {
            cfg.Map(poco => poco.Profession, typeof(string));
        });
    }
}

public partial class AuthorMappingProfile //: Mapper<AuthorCreateCommand, AuthorEntityResponse, AuthorEntity>
{
    public override Author ToEntity(AuthorCreateCommand e)
    {
        return new Author
        {
            FirstName = e.FirstName,
            LastName = e.LastName,
            Profession = e.Profession,
        };
    }
}


//[JsonSerializable(typeof(AuthorResponse))]
//public partial class AuthorSerializerContext : JsonSerializerContext { }