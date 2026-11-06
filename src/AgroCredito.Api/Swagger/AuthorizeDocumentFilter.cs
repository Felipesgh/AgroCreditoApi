using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AgroCredito.Api.Swagger;

/// <summary>Marca com o cadeado (Bearer) as rotas que exigem autenticação.</summary>
public sealed class AuthorizeDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument document, DocumentFilterContext context)
    {
        foreach (var description in context.ApiDescriptions)
        {
            var metadata = description.ActionDescriptor.EndpointMetadata;
            if (metadata is null)
                continue;

            if (metadata.OfType<IAllowAnonymous>().Any() || !metadata.OfType<IAuthorizeData>().Any())
                continue;

            if (string.IsNullOrWhiteSpace(description.HttpMethod)
                || !document.Paths.TryGetValue($"/{description.RelativePath}", out var path)
                || path is null
                || !path.Operations.TryGetValue(new HttpMethod(description.HttpMethod), out var operation))
                continue;

            operation.Security ??= [];
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        }
    }
}
