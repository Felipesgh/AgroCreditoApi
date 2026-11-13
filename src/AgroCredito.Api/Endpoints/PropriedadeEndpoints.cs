using System.Security.Claims;
using AgroCredito.Application.DTOs;
using AgroCredito.Application.Services;

namespace AgroCredito.Api.Endpoints;

public static class PropriedadeEndpoints
{
    public static IEndpointRouteBuilder MapPropriedadeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/propriedades")
            .WithTags("Propriedades")
            .RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal user, PropriedadeService service, CancellationToken ct) =>
                TypedResults.Ok(await service.ListAsync(user.GetProdutorId(), ct)))
            .WithName("ListarPropriedades")
            .WithSummary("Lista as propriedades do produtor autenticado")
            .Produces<IReadOnlyList<PropriedadeResponse>>()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/{id}", async (int id, ClaimsPrincipal user, PropriedadeService service, CancellationToken ct) =>
                TypedResults.Ok(await service.GetAsync(id, user.GetProdutorId(), ct)))
            .WithName("ObterPropriedade")
            .WithSummary("Detalha uma propriedade do produtor autenticado")
            .Produces<PropriedadeResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", async (PropriedadeRequest request, ClaimsPrincipal user, PropriedadeService service, CancellationToken ct) =>
            {
                var criada = await service.CreateAsync(user.GetProdutorId(), request, ct);
                return TypedResults.Created($"/api/propriedades/{criada.Id}", criada);
            })
            .WithName("CriarPropriedade")
            .WithSummary("Cadastra uma propriedade")
            .Produces<PropriedadeResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPut("/{id}", async (int id, PropriedadeRequest request, ClaimsPrincipal user, PropriedadeService service, CancellationToken ct) =>
                TypedResults.Ok(await service.UpdateAsync(id, user.GetProdutorId(), request, ct)))
            .WithName("AtualizarPropriedade")
            .WithSummary("Atualiza uma propriedade")
            .Produces<PropriedadeResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id}", async (int id, ClaimsPrincipal user, PropriedadeService service, CancellationToken ct) =>
            {
                await service.DeleteAsync(id, user.GetProdutorId(), ct);
                return TypedResults.NoContent();
            })
            .WithName("ExcluirPropriedade")
            .WithSummary("Exclui uma propriedade")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
