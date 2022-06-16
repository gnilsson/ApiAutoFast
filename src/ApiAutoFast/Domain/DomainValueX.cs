//using ApiAutoFast.Descriptive;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq.Expressions;
//using System.Reflection;

//namespace ApiAutoFast.Domain;

////internal class DomainValueX<TRequest, TDomain>
////{
////    internal class WithEntity<TEntity> : DomainValueX<TRequest, TDomain>
////    {

////    }

////    internal class WithResponse<TResponse> : DomainValueX<TRequest, TDomain>
////    {

////    }
////}

////internal sealed class Test : DomainValueX<string, Test>.WithEntity<string>.WithResponse<string>
////{

////}



//internal abstract class DomainValueX<TEntity, TDomain> where TDomain : DomainValueX<TEntity, TDomain>, new()
//{
//    static DomainValueX()
//    {
//        var ctor = typeof(TDomain)
//            .GetTypeInfo()
//            .DeclaredConstructors
//            .First();

//        var argsExp = Array.Empty<Expression>();
//        var newExp = Expression.New(ctor, argsExp);
//        var lambda = Expression.Lambda(typeof(Func<TDomain>), newExp);

//        _factory = (Func<TDomain>)lambda.Compile();
//    }

//    private static readonly Func<TDomain> _factory;

//    public TEntity EntityValue { get; private set; } = default!;

//    protected virtual bool TryValidateRequestConversion(TEntity? requestValue, [NotNullWhen(true)] out TEntity entityValue)
//    {
//        entityValue = requestValue!;
//        return requestValue is not null;
//    }

//    public static TDomain ConvertFromRequest(TEntity? request, Action<string, string> addError)
//    {
//        var domain = _factory();

//        if (domain.TryValidateRequestConversion(request, out var entityValue))
//        {
//            domain.EntityValue = entityValue;
//            return domain;
//        }

//        addError(typeof(TDomain).Name, "Error when converting request.");
//        return default!;
//    }

//    public static implicit operator DomainValueX<TEntity, TDomain>(TEntity entityValue) => From(entityValue);

//    public static implicit operator TEntity(DomainValueX<TEntity, TDomain> domain) => domain.EntityValue;

//    public static DomainValueX<TEntity, TDomain> From(TEntity entityValue)
//    {
//        var domain = _factory();
//        domain.EntityValue = entityValue;
//        return domain;
//    }

//    internal abstract class WithRequest<TRequest> : DomainValueX<TEntity, TDomain>
//    {
//        static WithRequest()
//        {
//            var m = typeof(WithRequest<>).GetMethod(nameof(TryValidateRequestConversion));
//        }

//        protected virtual bool TryValidateRequestConversion(TRequest? requestValue, [NotNullWhen(true)] out TEntity entityValue)
//        {
//            entityValue = default!;

//            if (requestValue is TEntity entityRequestValue)
//            {
//                entityValue = entityRequestValue;
//            }

//            return requestValue is not null;
//        }

//        public static TDomain ConvertFromRequest(TRequest? request, Action<string, string> addError)
//        {
//            var domain = _factory();

//            if (.TryValidateRequestConversion(request, out var entityValue))
//            {
//                domain.EntityValue = entityValue;
//                return domain;
//            }

//            addError(typeof(TDomain).Name, "Error when converting request.");
//            return default!;
//        }

//        internal abstract class WithResponse<TResponse> : DomainValueX<TEntity, TDomain>
//        {

//        }
//    }

//    //internal abstract class WithEntity<TEntity> : DomainValueX<TEntity, TDomain>
//    //{
//    //    public new TEntity EntityValue { get; } = default!;

//    //    protected virtual bool TryValidateRequestConversion(TEntity? requestValue, [NotNullWhen(true)] out TEntity entityValue)
//    //    {
//    //        entityValue = default!;

//    //        if (requestValue is TEntity entityRequestValue)
//    //        {
//    //            entityValue = entityRequestValue;
//    //        }

//    //        return requestValue is not null;
//    //    }

//    //    public static implicit operator WithEntity<TEntity>(TEntity entityValue) => From(entityValue);

//    //    public static implicit operator TEntity(WithEntity<TEntity> domain) => domain.EntityValue;

//    //    public static WithEntity<TEntity> From(TEntity entityValue)
//    //    {
//    //        var domain = _factory();
//    //        domain.EntityValue = entityValue;
//    //        return domain;
//    //    }

//    //    internal abstract class WithResponse<TResponse> : WithEntity<TEntity>
//    //    {

//    //    }

//    //    //internal abstract class WithRequestResponseType : WithEntity<TEntity>
//    //    //{

//    //    //}
//    //}


//}

//internal sealed class Test : DomainValueX<string, Test>.WithRequest<string>
//{

//}

//internal sealed class Test2 : DomainValueX<string, Test2>
//{

//}