﻿namespace TX.RMC.Api.Swagger.Filters;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class EnumTypesDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var path in swaggerDoc.Paths.Values)
        {
            foreach (var operation in path.Operations.Values)
            {
                foreach (var parameter in operation.Parameters)
                {
                    var schemaReferenceId = parameter.Schema.Reference?.Id;

                    if (string.IsNullOrEmpty(schemaReferenceId)) continue;

                    var schema = context.SchemaRepository.Schemas[schemaReferenceId];

                    if (schema.Enum == null || schema.Enum.Count == 0) continue;

                    parameter.Description += "<p>Variants:</p>";

                    int cutStart = schema.Description.IndexOf("<ul>");

                    int cutEnd = schema.Description.IndexOf("</ul>") + 5;

                    parameter.Description += schema.Description
                        .Substring(cutStart, cutEnd - cutStart);
                }
            }
        }
    }
}
