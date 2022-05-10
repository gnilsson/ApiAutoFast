//using Mapster;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using YamlDotNet.Core.Tokens;

//namespace ApiAutoFast.Sample.Server.Database;

//public partial class MappingRegister
//{
//    public MappingRegister()
//    {
//        _extendRegisterResponses = true;
//    }

//    static partial void OnExtendRegisterResponses(AdaptAttributeBuilder aab)
//    {
//        aab.ForType<Author>(cfg =>
//        {
//            cfg.Map(poco => poco.Profession, typeof(string));
//        });
//    }
//}


//public partial class AuthorMappingProfile //: Mapper<AuthorCreateCommand, AuthorEntityResponse, AuthorEntity>
//{
////private readonly bool _onOverrideUpdateEntity = false;

////partial void OnOverrideUpdateEntity(ref Author originalEntity, AuthorModifyCommand e)
////{
////    return;
////}

//// default impl for toentity?
//    public override Author ToEntity(AuthorCreateCommand e)
//    {
//        return new Author
//        {
//            FirstName = e.FirstName,
//            LastName = e.LastName,
//            Profession = e.Profession,
//        };
//    }
//}

//public partial class GetByIdAuthorEndpoint
//{
//    public override async Task OnBeforeHandleAsync(AuthorGetByIdRequest req)
//{
//        var result = await _dbContext.Authors.FindAsync((Identifier)req.Id);

//        await base.OnBeforeHandleAsync(req);
//    }


//    //public override async Task<AuthorResponse> ExecuteAsync(AuthorGetByIdRequest req, CancellationToken ct)
//    //{
//    //    var author = await _dbContext.Authors.SingleOrDefaultAsync(x => x.Id == req.Id, ct);

//    //    if(author is null)
//    //    {
//    //        await SendNotFoundAsync(ct);
//    //    }

//    //    var response = Map.FromEntity(author);

//    //    await SendOkAsync(response, ct);
//    //}
//}


////[JsonSerializable(typeof(AuthorResponse))]
////public partial class AuthorSerializerContext : JsonSerializerContext { }

//public partial class CreateAuthorEndpoint
//{
//    private readonly IService _service;

//    public CreateAuthorEndpoint(AutoFastSampleDbContext dbContext, IService service)
//{
//        _dbContext = dbContext;
//        _service = service;
//    }

//    //partial void ExtendConfigure()
//    //{
//    //}

//    //public override async Task OnBeforeHandleAsync(AuthorCreateCommand req)
//    //{
//    //    _service.Log();

//    //    //var count = await _dbContext.Authors.FindAsync(new[] { "" });

//    //    //var add = await _dbContext.Authors.AddAsync(null);

//    // //   Map

//    //    //Console.WriteLine($"Hej! {count}st.");

//    //  //  SendCreatedAtAsync<GetByIdAuthorEndpoint>("", default, Http.GET, cancellation: CancellationToken.None);

//    //    await base.OnBeforeHandleAsync(req);
//    //}
//}


//public interface IService
//{
//    public void Log();
//}

//public class FooService : IService
//{
//    public void Log()
//    {
//        Console.WriteLine("hej");
//    }
//}


using ApiAutoFast.Sample.Server.Database;
using FastEndpoints;
using YamlDotNet.Core.Tokens;

public partial class Lul
{
    void Hmm(string hm)
    {
        Hej(ref hm);
    }

    partial void Hej(ref string ah)
    {
return;
}
}
public partial class Lul
{
partial void Hej(ref string ah);
}

//public partial class AuthorMappingProfile : Mapper<object, AuthorResponse, object>
//{
//private readonly bool _onOverrideUpdateEntity = false;

//    partial void OnOverrideUpdateEntity(ref object originalEntity, object e);

//    public override AuthorResponse FromEntity(object e)
//{
//return e.AdaptToResponse();
//}
//public object UpdateEntity(object originalEntity, object e)
//    {
//        if (_onOverrideUpdateEntity)
//        {
//            OnOverrideUpdateEntity(ref originalEntity, e);
//            return originalEntity;
//        }

//        return originalEntity;
//    }
//}
