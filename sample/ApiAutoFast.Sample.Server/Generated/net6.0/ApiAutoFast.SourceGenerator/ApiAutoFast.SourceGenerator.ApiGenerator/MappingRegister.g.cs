
using Mapster;
using ApiAutoFast;

namespace ApiAutoFast.Sample.Server;

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

        TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);
        TypeAdapterConfig.GlobalSettings.Default.MaxDepth(2);
        TypeAdapterConfig.GlobalSettings.Default.ShallowCopyForSameType(true);
        TypeAdapterConfig.GlobalSettings.Default.EnumMappingStrategy(EnumMappingStrategy.ByName);
        TypeAdapterConfig.GlobalSettings.Default.AddDestinationTransform(DestinationTransform.EmptyCollectionIfNull);

        TypeAdapterConfig.GlobalSettings
            .When((src, dest, map) => src.GetInterface(nameof(IEntity<IIdentifier>)) is not null)
            .Map(nameof(IEntity<IIdentifier>.CreatedDateTime), (IEntity<IIdentifier> e) => e.CreatedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"))
            .Map(nameof(IEntity<IIdentifier>.ModifiedDateTime), (IEntity<IIdentifier> e) => e.ModifiedDateTime.ToString("dddd, dd MMMM yyyy HH:mm"));
            TypeAdapterConfig<Title, string>.NewConfig().MapWith(x => x.EntityValue);

            ExtendRegister(config);

            config.GenerateMapper("[name]Mapper")
                .ForType<Blog>();
        }
    }

public static class AdaptAttributeBuilderExtensions
{
    public static AdaptAttributeBuilder ForTypeDefaultValues(this AdaptAttributeBuilder aab)
    {
        return aab
                .ForType<Blog>(cfg =>
                {
                    cfg.Map(poco => poco.Id, typeof(string));
                    cfg.Map(poco => poco.CreatedDateTime, typeof(string));
                    cfg.Map(poco => poco.ModifiedDateTime, typeof(string));
                    cfg.Map(poco => poco.Title, typeof(string));
                });
    }
}
