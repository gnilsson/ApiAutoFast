using MR.AspNetCore.Pagination;
using NSwag.Generation.Processors.Contexts;
using NSwag.Generation.Processors;
using NSwag;
using Microsoft.Extensions.Options;
using NJsonSchema;

namespace ApiAutoFast.Sample.Server;

internal sealed class PaginationOperationProcessor : IOperationProcessor
{
    private readonly PaginationOptions _paginationOptions;

    public PaginationOperationProcessor()
    {
        _paginationOptions = new PaginationOptions();
    }

    public bool Process(OperationProcessorContext context)
    {

        //var intSchema = context.SchemaGenerator.Generate(typeof(int), context.SchemaResolver);

        //if (PaginationActionDetector.IsKeysetPaginationResultAction(context.MethodInfo, out _))
        //{
        //    CreateParameter(_paginationOptions.FirstQueryParameterName, "true if you want the first page", boolSchema);
        //    CreateParameter(_paginationOptions.BeforeQueryParameterName, "Id of the reference entity you want results before");
        //    CreateParameter(_paginationOptions.AfterQueryParameterName, "Id of the reference entity you want results after");
        //    CreateParameter(_paginationOptions.LastQueryParameterName, "true if you want the last page", boolSchema);
        //}
        //else if (PaginationActionDetector.IsOffsetPaginationResultAction(context.MethodInfo, out _))
        //{
        //    CreateParameter(_paginationOptions.PageQueryParameterName, "The page", intSchema);
        //}

        var boolSchema = context.SchemaGenerator.Generate(typeof(bool), context.SchemaResolver);

        CreateParameter(_paginationOptions.FirstQueryParameterName, "true if you want the first page", boolSchema);
        CreateParameter(_paginationOptions.BeforeQueryParameterName, "Id of the reference entity you want results before");
        CreateParameter(_paginationOptions.AfterQueryParameterName, "Id of the reference entity you want results after");
        CreateParameter(_paginationOptions.LastQueryParameterName, "true if you want the last page", boolSchema);

        void CreateParameter(string name, string description, JsonSchema? schema = null)
        {
            context.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
            {
                IsRequired = false,
                Kind = OpenApiParameterKind.Query,
                Name = name,
                Description = description,
                Schema = schema,
            });
        }
        return true;
    }
}
