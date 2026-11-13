using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AgroCredito.Api.Swagger;

public sealed class AuthorizeDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument document, DocumentFilterContext context)
    {
        foreach (var description in context.ApiDescriptions)
        {
            var metadata = description.ActionDescriptor.EndpointMetadata;

            if (metadata is null)
                continue;

            if (metadata.OfType<IAllowAnonymous>().Any()
                || !metadata.OfType<IAuthorizeData>().Any())
                continue;

            if (string.IsNullOrWhiteSpace(description.HttpMethod)
                || string.IsNullOrWhiteSpace(description.RelativePath))
                continue;

            var caminhoApi = NormalizarCaminho(description.RelativePath);

            var caminhoSwagger = document.Paths.FirstOrDefault(
                p => NormalizarCaminho(p.Key) == caminhoApi);

            if (caminhoSwagger.Value is null)
                continue;

            if (!caminhoSwagger.Value.Operations.TryGetValue(
                new HttpMethod(description.HttpMethod), out var operation))
                continue;

            operation.Security ??= [];

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        }
    }

    private static string NormalizarCaminho(string caminho)
    {
        var caminhoSemQuery = caminho.Split('?')[0];
        return "/" + caminhoSemQuery.Trim('/');
    }
}
