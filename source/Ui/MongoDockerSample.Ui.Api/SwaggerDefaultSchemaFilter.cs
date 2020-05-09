using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Collections.Generic;

namespace MongoDockerSample.Ui.Api
{
    internal class SwaggerDefaultSchemaFilter : ISchemaFilter
    {
        private static readonly HashSet<string> ignoreProperties = new HashSet<string>(
            typeof(System.Exception).GetProperties().Where(p => p.Name != nameof(System.Exception.Message))
                .Select(p => p.Name));

        private static readonly string[] necessarySchemas = { "HttpStatusCode" };

        private const string WebApiNamespace = "MongoDockerSample.Ui.Api";

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type != null && schema.Properties != null && schema.Properties.Count > 0)
            {
                if (typeof(System.Exception).IsAssignableFrom(context.Type))
                {
                    foreach (var ignoreProperty in ignoreProperties)
                    {
                        var keyCheck = schema.Properties
                            .Keys.Where(k => k.ToLowerInvariant() == ignoreProperty.ToLowerInvariant());

                        if (keyCheck.Count() == 1)
                        {
                            schema.Properties.Remove(keyCheck.Single());
                        }
                    }
                }
                else if (!context.Type.FullName.StartsWith(WebApiNamespace))
                {
                    var propertiesToRemove = schema.Properties.Keys.ToList();

                    foreach (var prop in propertiesToRemove)
                    {
                        schema.Properties.Remove(prop);
                    }

                    var schemas = context.SchemaRepository.Schemas
                        .Where(s => SchemaValidForRemove(s))
                        .Select(s => s.Key);

                    foreach (var schemaToDelete in schemas)
                    {
                        context.SchemaRepository.Schemas.Remove(schemaToDelete);
                    }
                }
            }
        }

        private bool SchemaValidForRemove(KeyValuePair<string, OpenApiSchema> schema)
            => !necessarySchemas.Any(n => n.ToLowerInvariant() == schema.Key.ToLowerInvariant())
                && (schema.Value.Properties == null || !schema.Value.Properties.Any());
    }
}