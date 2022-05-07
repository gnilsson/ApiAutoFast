using FastEndpoints;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
    //private readonly bool _onOverrideUpdateEntity = false;

    //public partial Author OnOverrideUpdateEntity(Author originalEntity, AuthorModifyCommand e)
    //{
    //    return null;
    //}

    // default impl for toentity?
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

public partial class GetByIdAuthorEndpoint
{
    public override Task OnBeforeHandleAsync(AuthorGetByIdRequest req)
    {
        return base.OnBeforeHandleAsync(req);
    }


    //public override async Task<AuthorResponse> ExecuteAsync(AuthorGetByIdRequest req, CancellationToken ct)
    //{
    //    var author = await _dbContext.Authors.SingleOrDefaultAsync(x => x.Id == req.Id, ct);

    //    if(author is null)
    //    {
    //        await SendNotFoundAsync(ct);
    //    }

    //    var response = Map.FromEntity(author);

    //    await SendOkAsync(response, ct);
    //}
}


//[JsonSerializable(typeof(AuthorResponse))]
//public partial class AuthorSerializerContext : JsonSerializerContext { }

public class Ah : IEntityMapper
{

}

public partial class CreateAuthorEndpoint
{
    private readonly IService _service;

    public CreateAuthorEndpoint(AutoFastSampleDbContext dbContext, IService service)
    {
        _dbContext = dbContext;
        _service = service;
    }

    partial void ExtendConfigure()
    {
    }

    public override async Task OnBeforeHandleAsync(AuthorCreateCommand req)
    {
        _service.Log();

        var count = await _dbContext.Authors.FindAsync(new[] { "" });

        //var add = await _dbContext.Authors.AddAsync(null);

     //   Map

        Console.WriteLine($"Hej! {count}st.");

      //  SendCreatedAtAsync<GetByIdAuthorEndpoint>("", default, Http.GET, cancellation: CancellationToken.None);

        await base.OnBeforeHandleAsync(req);
    }

    public override void OnBeforeValidate(AuthorCreateCommand req)
    {
        base.OnBeforeValidate(req);
    }
}


public interface IService
{
    public void Log();
}

public class FooService : IService
{
    public void Log()
    {
        Console.WriteLine("hej");
    }
}
