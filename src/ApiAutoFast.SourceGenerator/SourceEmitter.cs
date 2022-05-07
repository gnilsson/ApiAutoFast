//using System.Text;

//namespace ApiAutoFast;

//public static class SourceEmitter
//{
//    public const string AutoFastContext = @"
//";
//    private const string JsonAuthor = "Author";
//    public const string AutoFastJson = @$"
//using System.Text.Json.Serialization;
//using ApiAutoFast.Sample.Server.Database;

//namespace ApiAutoFast;

//[JsonSerializable(typeof({JsonAuthor}Response))]
//public partial class {JsonAuthor}SerializerContext : JsonSerializerContext {{ }}";

//    public const string AutoFastAttribute = @"
//namespace ApiAutoFast
//{
//    [System.AttributeUsage(System.AttributeTargets.Class)]
//    internal class AutoFastAttribute : System.Attribute
//    {
//    }
//}";

//    public const string AutoFastContextAttribute = @"
//namespace ApiAutoFast
//{
//    [System.AttributeUsage(System.AttributeTargets.Class)]
//    internal class AutoFastContextAttribute : System.Attribute
//    {
//    }
//}";

//    public static string GenerateEndpoints(EntityDbSetsToGenerate entityDbSetsToGenerate)
//    {
//        var sb = new StringBuilder();

//        var newNamespace = CreateNewNamespace(entityDbSetsToGenerate.NameSpace, ".Endpoints");
//        var mappingNamespace = CreateNewNamespace(entityDbSetsToGenerate.NameSpace, ".Mapping");
//        var databaseNamespace = CreateNewNamespace(entityDbSetsToGenerate.NameSpace, ".Database");

//        sb.Append(@"
//using ApiAutoFast;
//using FastEndpoints;
//using System.Threading;
//using System.Threading.Tasks;
//using Microsoft.EntityFrameworkCore;
//using ").Append(mappingNamespace).Append(@";
//using ").Append(databaseNamespace).Append(@";

//namespace ").Append(newNamespace).Append(@";
//");
//        foreach (var entityName in entityDbSetsToGenerate.Names)
//        {
//            var baseName = entityName.Split('.').Last();
//            var response = $"{baseName}Response";
//            var route = $"{baseName}s".ToLower();
//            var request = $"{baseName}Request";
//            var command = $"{baseName}Command";
//            var mappingProfile = $"{baseName}MappingProfile";

//            sb.Append(@"

//public partial class Get").Append(baseName).Append(@"Endpoint : Endpoint<").Append(request).Append(@", PaginatedResponse<").Append(response).Append(@">, ").Append(mappingProfile).Append(@">
//{
//    partial void ExtendConfigure();
//    private bool _overrideConfigure = false;
//    private readonly AutoFastDbContext _dbContext;

//    public Get").Append(baseName).Append(@"Endpoint(AutoFastDbContext dbContext)
//    {
//        _dbContext = dbContext;
//    }

//    public override void Configure()
//    {
//        if (_overrideConfigure is false)
//        {
//            Verbs(Http.GET);
//            Routes(""/").Append(route).Append(@""");

//            AllowAnonymous();
//        }

//        ExtendConfigure();
//    }

//    public override async Task HandleAsync(").Append(request).Append(@" req, CancellationToken ct)
//    {
//        var result = await _dbContext.").Append(baseName).Append(@"s.Where(x => true).ToArrayAsync(ct);

//        if (result.Length == 0)
//        {
//           await SendNotFoundAsync(ct);
//           return;
//        }

//        var response = result.Select(x => Map.FromEntity(x));

//        var pag = new PaginatedResponse<").Append(response).Append(@"> { Data = response };

//        await SendOkAsync(pag, ct);
//    }
//}
//");
//            sb.Append(@"
//public partial class Create").Append(baseName).Append(@"Endpoint : Endpoint<").Append(command).Append(@", ").Append(response).Append(@", ").Append(mappingProfile).Append(@">
//{
//    partial void ExtendConfigure();
//    private bool _overrideConfigure = false;
//    private readonly AutoFastDbContext _dbContext;

//    public Create").Append(baseName).Append(@"Endpoint(AutoFastDbContext dbContext)
//    {
//        _dbContext = dbContext;
//    }

//    public override void Configure()
//    {
//        if (_overrideConfigure is false)
//        {
//            Verbs(Http.POST);
//            Routes(""/").Append(route).Append(@""");

//            AllowAnonymous();
//        }

//        ExtendConfigure();
//    }

//    public override async Task HandleAsync(").Append(command).Append(@" req, CancellationToken ct)
//    {
//        var entity = Map.ToEntity(req);

//        await _dbContext.AddAsync(entity, ct);

//        await _dbContext.SaveChangesAsync(ct);

//        var response = Map.FromEntity(entity);

//        await SendOkAsync(response, ct);
//    }
//}
//");
//        }

//        return sb.ToString();
//    }
//    //            SerializerContext(").Append("new ").Append(baseName).Append(@"SerializerContext());
//    //            SerializerContext(").Append("new ").Append(baseName).Append(@"SerializerContext());
//    //    using ").Append(entityDbSetsToGenerate.NameSpace).Append(@";
//    //using ").Append(modelsNamespace).Append(@";

//    public static string GenerateMappingProfiles(EntityDbSetsToGenerate entityDbSetsToGenerate)
//    {
//        var sb = new StringBuilder();

//        //var newNamespace = CreateNewNamespace(entityDbSetsToGenerate.NameSpace, ".Mapping");
//        //var modelsNamespace = CreateNewNamespace(entityDbSetsToGenerate.NameSpace, ".Models");

//        //[JsonSerializable(typeof(").Append(response).Append(@"))]
//        //public partial class ").Append(baseName).Append(@"SerializerContext : JsonSerializerContext { }
//        // using System.Text.Json.Serialization;

//        sb.Append(@"
//using FastEndpoints;


//namespace ").Append(entityDbSetsToGenerate.NameSpace).Append(@";
//");
//        foreach (var entityName in entityDbSetsToGenerate.Names)
//        {
//            var baseName = entityName.Split('.').Last();
//            var response = $"{baseName}Response";
//            var command = $"{baseName}Command";
//            var request = $"{baseName}Request";
//            var mappingProfile = $"{baseName}MappingProfile";

//            sb.Append(@"

//public partial class ").Append(request).Append(@" { };").Append(@"

//public partial class ").Append(command).Append(@" { };").Append(@"

//public partial class ").Append(mappingProfile).Append(@" : Mapper<").Append(command).Append(@", ").Append(response).Append(", ").Append(@"Entity.").Append(baseName).Append(@">
//{
//    public override ").Append(response).Append(@" FromEntity(").Append(@"Entity.").Append(baseName).Append(@" e)
//    {
//        return e.AdaptToResponse();
//    }
//}");
//        }

//        return sb.ToString();
//    }

//    public static string GenerateDbContext(EntityDbSetsToGenerate entityDbSetsToGenerate)
//    {
//        var sb = new StringBuilder();

//        sb.Append(@"
//using ApiAutoFast;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.ChangeTracking;

//namespace ").Append(entityDbSetsToGenerate.NameSpace).Append(@";

//public partial class AutoFastDbContext : DbContext
//{
//    private static readonly Type[] _entityTypes;

//    static AutoFastDbContext()
//    {
//        _entityTypes = AutoFastDbContextHelper.GetEntityTypes<AutoFastDbContext>();
//    }

//    public AutoFastDbContext(DbContextOptions<AutoFastDbContext> options) : base(options) { }

//    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
//    {
//        AutoFastDbContextHelper.UpdateModifiedDateTime(ChangeTracker.Entries());

//        return await base.SaveChangesAsync(cancellationToken);
//    }

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        AutoFastDbContextHelper.BuildEntities(modelBuilder, _entityTypes);
//    }
//");
//        foreach (var entityName in entityDbSetsToGenerate.Names)
//        {
//            var baseName = entityName.Split('.').Last();

//            sb.Append(@"
//    public DbSet<").Append(entityName).Append("> ").Append(baseName).Append(@"s { get; init; } = default!;");
//        }

//        sb.Append(@"
//}
//");

//        return sb.ToString();
//    }




//    public static string GenerateMappingRegister(EntityDbSetsToGenerate entityDbSetsToGenerate)
//    {
//        var sb = new StringBuilder();

//        var newNamespace = CreateNewNamespace(entityDbSetsToGenerate.NameSpace, ".Mapping");

//        sb.Append(@"
//using Mapster;
//using ApiAutoFast.Domain;
//using ").Append(entityDbSetsToGenerate.NameSpace).Append(@";

//namespace ").Append(newNamespace).Append(@";

//public partial class MappingRegister : ICodeGenerationRegister
//{
//    private bool _overrideRegisterResponses = false;
//    private bool _extendRegisterResponses = false;
//    partial void OnOverrideRegisterResponses(AdaptAttributeBuilder aab);
//    partial void OnExtendRegisterResponses(AdaptAttributeBuilder aab);

//    public void Register(CodeGenerationConfig config)
//    {
//        var aab = config.AdaptTo(""[name]Response"");

//        if (_overrideRegisterResponses)
//        {
//            OnOverrideRegisterResponses(aab);
//        }
//        else if (_extendRegisterResponses)
//        {
//            aab.ForTypeDefaultValues();

//            OnExtendRegisterResponses(aab);
//        }
//        else
//        {
//            aab.ForTypeDefaultValues();
//        }

//        TypeAdapterConfig.GlobalSettings.Default.EnumMappingStrategy(EnumMappingStrategy.ByName);

//        TypeAdapterConfig.GlobalSettings
//            .When((src, dest, map) => src.GetInterface(nameof(IEntity)) is not null)
//            .Map(nameof(IEntity.CreatedDateTime), (IEntity e) => e.CreatedDateTime.ToString(""dddd, dd MMMM yyyy HH: mm""))
//            .Map(nameof(IEntity.ModifiedDateTime), (IEntity e) => e.ModifiedDateTime.ToString(""dddd, dd MMMM yyyy HH:mm""));

//        RegisterMappers(config);
//    }

//    private static void RegisterMappers(CodeGenerationConfig config)
//    {
//        config.GenerateMapper(""[name]Mapper"")");

//        foreach (var name in entityDbSetsToGenerate.Names)
//        {
//            sb.Append(@"
//        .ForType<").Append(name).Append(@">()");
//        }

//        sb.Append(@";").Append(@"
//    }
//}

//public static class AdaptAttributeBuilderExtensions
//{
//    public static AdaptAttributeBuilder ForTypeDefaultValues(this AdaptAttributeBuilder aab)
//    {
//        aab");
//        foreach (var name in entityDbSetsToGenerate.Names)
//        {
//            sb.Append(@"
//        .ForType<").Append(name).Append(@">(cfg =>
//        {
//            cfg.Map(poco => poco.Id, typeof(string));
//            cfg.Map(poco => poco.CreatedDateTime, typeof(string));
//            cfg.Map(poco => poco.ModifiedDateTime, typeof(string));
//            // todo: enums
//            // foreach enum property in entity write cfg.map => enum, typeof(string)
//        })");
//        }
//        sb.Append(@";

//        return aab;
//    }
//}
//");

//        return sb.ToString();
//    }

//    private static string CreateNewNamespace(string oldNamespace, string newEnding)
//    {
//        var dots = oldNamespace.Split('.');
//        var baseNamespace = string.Join(".", dots.Take(dots.Length - 1));
//        var newNamespace = $"{baseNamespace}{newEnding}";
//        return newNamespace;
//    }
//}