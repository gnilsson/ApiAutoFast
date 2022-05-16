
using Mapster;
using ApiAutoFast;

namespace ApiAutoFast.Sample.Server.Database;

public partial class MappingRegister : ICodeGenerationRegister
{
    private bool _overrideRegisterResponses = false;
    private bool _extendRegisterResponses = false;

    static partial void OnOverrideRegisterResponses(AdaptAttributeBuilder aab);
    static partial void OnExtendRegisterResponses(AdaptAttributeBuilder aab);
    static partial void ExtendRegister(CodeGenerationConfig config);
    static partial void RegisterMappers(CodeGenerationConfig config);

    public void Register(CodeGenerationConfig config)
    {
        var aab = config.AdaptTo("[name]Response");

        if (_overrideRegisterResponses)
        {
            OnOverrideRegisterResponses(aab);
        }
        else if (_extendRegisterResponses)
        {
            aab.ForTypeDefaultValues();

            OnExtendRegisterResponses(aab);
        }
        else
        {
            aab.ForTypeDefaultValues();
        }

        TypeAdapterConfig.GlobalSettings.Default.EnumMappingStrategy(EnumMappingStrategy.ByName);

        TypeAdapterConfig.GlobalSettings
            .When((src, dest, map) => src.GetInterface(nameof(IEntity)) is not null)
            .Map(nameof(IEntity.CreatedDateTime), (IEntity e) => e.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"))
            .Map(nameof(IEntity.ModifiedDateTime), (IEntity e) => e.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"));

        TypeAdapterConfig<Title, string>.NewConfig().MapWith(x => x.EntityValue);
        TypeAdapterConfig<PublicationDateTime, string>.NewConfig().MapWith(x => x.ToString());
        TypeAdapterConfig<Description, string>.NewConfig().MapWith(x => x.EntityValue);
        TypeAdapterConfig<PostType, string>.NewConfig().MapWith(x => x.ToString());
        TypeAdapterConfig<LikeCount, int>.NewConfig().MapWith(x => x.EntityValue);

        ExtendRegister(config);

        config.GenerateMapper("[name]Mapper")
            .ForType<Post>();
    }
}

public static class AdaptAttributeBuilderExtensions
{
    public static AdaptAttributeBuilder ForTypeDefaultValues(this AdaptAttributeBuilder aab)
    {
        return aab
            .ForType<Post>(cfg =>
            {
                cfg.Map(poco => poco.Id, typeof(string));
                cfg.Map(poco => poco.CreatedDateTime, typeof(string));
                cfg.Map(poco => poco.ModifiedDateTime, typeof(string));
                cfg.Map(poco => poco.Title, typeof(string));
                cfg.Map(poco => poco.PublicationDateTime, typeof(string));
                cfg.Map(poco => poco.Description, typeof(string));
                cfg.Map(poco => poco.PostType, typeof(string));
                cfg.Map(poco => poco.LikeCount, typeof(int));
            });
    }
}
