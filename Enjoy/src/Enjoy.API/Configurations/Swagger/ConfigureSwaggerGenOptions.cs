using System.Collections.Generic;
using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Enjoy.Presentation;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Enjoy.API.Configurations.Swagger;

public sealed class ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
    : IConfigureNamedOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = $"Enjoy API · {description.GroupName}",
                Version = description.ApiVersion.ToString(),
                Description =
                    "Composition root: Application, Infrastructure, Persistence, and Presentation."
            });
        }

        options.ResolveConflictingActions(descriptions => descriptions.First());

        var apiXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var apiPath = Path.Combine(AppContext.BaseDirectory, apiXml);
        if (File.Exists(apiPath))
            options.IncludeXmlComments(apiPath, includeControllerXmlComments: true);

        var presentationXml = $"{typeof(AssemblyReference).Assembly.GetName().Name}.xml";
        var presentationPath = Path.Combine(AppContext.BaseDirectory, presentationXml);
        if (File.Exists(presentationPath))
            options.IncludeXmlComments(presentationPath, includeControllerXmlComments: true);

        options.CustomSchemaIds(type => type.FullName?.Replace("+", ".", StringComparison.Ordinal));
        options.DescribeAllParametersInCamelCase();

        options.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Description =
                    "Paste the JWT only (without the word Bearer). Swagger sends it as Authorization: Bearer {token}.",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document, null)] = []
        });
    }

    public void Configure(string? name, SwaggerGenOptions options) => Configure(options);
}
