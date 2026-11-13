using System.Security.Claims;
using AgroCredito.Application.DTOs;
using AgroCredito.Application.Services;

namespace AgroCredito.Api.Endpoints;

public static class ProdutorEndpoints
{
    public static IEndpointRouteBuilder MapProdutorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/produtores")
            .WithTags("Produtores")
            .RequireAuthorization();

        group.MapGet("/me", async (ClaimsPrincipal user, AuthService auth, CancellationToken ct) =>
                TypedResults.Ok(await auth.ObterPerfilAsync(user.GetProdutorId(), ct)))
            .WithName("MeuPerfil")
            .WithSummary("Dados do produtor autenticado")
            .Produces<ProdutorResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }
}
