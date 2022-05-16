

//using Mapster;
//using ApiAutoFast;

//namespace ApiAutoFast.Sample.Server.Database;

//internal sealed class MappingRegisterTest : ICodeGenerationRegister
//{
//    public void Register(CodeGenerationConfig config)
//    {
//        var aab = config.AdaptTo("[name]Response");


//        TypeAdapterConfig.GlobalSettings.Default.EnumMappingStrategy(EnumMappingStrategy.ByName);

//        TypeAdapterConfig.GlobalSettings
//            .When((src, dest, map) => src.GetInterface(nameof(IEntity)) is not null)
//            .Map(nameof(IEntity.CreatedDateTime), (IEntity e) => e.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"))
//            .Map(nameof(IEntity.ModifiedDateTime), (IEntity e) => e.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"));
//        //.Map(nameof(Post.LikeCount), (Post e) => e.LikeCount.EntityValue);

//        TypeAdapterConfig<LikeCount, int>.NewConfig().MapWith(x => x.EntityValue);

//        config.GenerateMapper("[name]Mapper")
//            .ForType<Post>();
//            //.ForType<Test>();
//    }
//}

//public static class AdaptAttributeBuilderExtensions2
//{
//    public static AdaptAttributeBuilder ForTypeDefaultValues(this AdaptAttributeBuilder aab)
//    {
//        return aab
//            .ForType<Post>(cfg =>
//            {
//                cfg.Map(poco => poco.LikeCount, typeof(int));
//                //     cfg.Map(poco => poco.Title, x => x.Title.ToString());
//            });
//    }
//}