using ApiAutoFast.SourceGenerator.Configuration.Enums;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct RequestEndpointPair
{
    internal RequestEndpointPair(RequestModelTarget requestModel, PropertyTarget propertyTarget, EndpointTargetType endpointTarget, string httpVerb)
    {
        RequestModel = requestModel;
        PropertyTarget = propertyTarget;
        EndpointTarget = endpointTarget;
        HttpVerb = httpVerb;
    }

    internal readonly RequestModelTarget RequestModel { get; }
    internal readonly PropertyTarget PropertyTarget { get; }
    internal readonly EndpointTargetType EndpointTarget { get; }
    internal readonly string HttpVerb { get; }
}
