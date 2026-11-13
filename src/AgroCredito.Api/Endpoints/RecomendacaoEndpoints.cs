using System.Security.Claims;
using AgroCredito.Application.DTOs;
using AgroCredito.Application.Services;

namespace AgroCredito.Api.Endpoints;

public static class RecomendacaoEndpoints
{
    public static IEndpointRouteBuilder MapRecomendacaoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/propriedades/{propriedadeId}/recomendacoes")
            .WithTags("Recomendações")
            .RequireAuthorization();

        group.MapGet("/", async (int propriedadeId, ClaimsPrincipal user, RecomendacaoService service, CancellationToken ct) =>
                TypedResults.Ok(await service.RecomendarAsync(propriedadeId, user.GetProdutorId(), ct)))
            .WithName("RecomendarOportunidades")
            .WithSummary("Oportunidades compatíveis com o perfil da propriedade")
            .WithDescription("Resultado informativo: não garante aprovação de crédito. Confirme as condições na fonte oficial.")
            .Produces<RecomendacaoResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
