using ApiAutoFast.SourceGenerator.Configuration.Enums;

namespace ApiAutoFast.SourceGenerator.Configuration;

internal readonly struct RequestEndpointPair
{
    internal RequestEndpointPair(RequestModelTarget requestModel, EndpointTargetType endpointTarget, string httpVerb)
    {
        RequestModel = requestModel;
        EndpointTarget = endpointTarget;
        HttpVerb = httpVerb;
    }

    internal readonly RequestModelTarget RequestModel { get; }
    internal readonly EndpointTargetType EndpointTarget { get; }
    internal readonly string HttpVerb { get; }
}
