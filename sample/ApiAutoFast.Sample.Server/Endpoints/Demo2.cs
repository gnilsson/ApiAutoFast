//using FastEndpoints;
//using System.Text.Json.Serialization;
//using ApiAutoFast.Sample.Server.Database;

//namespace ApiAutoFast.Sample.Server.Mapping;

//[JsonSerializable(typeof(AuthorResponse))]
//public partial class AuthorSerializerContext : JsonSerializerContext { }

//public partial class AuthorRequest { };

//public partial class AuthorCommand { };

//public partial class AuthorMappingProfile : Mapper<AuthorCommand, AuthorResponse, Entity.Author>
//{
//    public override AuthorResponse FromEntity(Entity.Author e)
//    {
//        return e.AdaptToResponse();
//    }
//}


//using FastEndpoints;
//using System.Text.Json.Serialization;


//namespace ApiAutoFast.Sample.Server.Database;


//[JsonSerializable(typeof(AuthorResponse))]
//public partial class AuthorSerializerCtx : JsonSerializerContext { }

//public partial class AuthorRequest { };

//public partial class AuthorCommand { };

//public partial class AuthorMappingProfile : Mapper<AuthorCommand, AuthorResponse, Entity.Author>
//{
//    public override AuthorResponse FromEntity(Entity.Author e)
//    {
//        return e.AdaptToResponse();
//    }
//}

