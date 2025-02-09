using DataConnectorLibraryProject.Enums;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApiProject.ExtendSwagerUI
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(DbContextType))
            {
                schema.Type = "string";
                schema.Enum = new List<IOpenApiAny> { new OpenApiString("sql"), new OpenApiString("mongo") };
            }
        }
    }
}
