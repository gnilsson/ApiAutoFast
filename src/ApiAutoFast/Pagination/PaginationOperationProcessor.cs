using ApiAutoFast.Descriptive;
using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace ApiAutoFast.Pagination;

public sealed class KeysetPaginationOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var boolSchema = context.SchemaGenerator.Generate(typeof(bool), context.SchemaResolver);

        CreateParameter(QueryParameterText.First, "true if you want the first page", boolSchema);
        CreateParameter(QueryParameterText.Before, "Id of the reference entity you want results before");
        CreateParameter(QueryParameterText.After, "Id of the reference entity you want results after");
        CreateParameter(QueryParameterText.Last, "true if you want the last page", boolSchema);

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
