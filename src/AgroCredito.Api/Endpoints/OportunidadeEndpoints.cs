using AgroCredito.Application.DTOs;
using AgroCredito.Application.Services;
using AgroCredito.Domain.Enums;

namespace AgroCredito.Api.Endpoints;

public static class OportunidadeEndpoints
{
    public static IEndpointRouteBuilder MapOportunidadeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/oportunidades").WithTags("Oportunidades");

        group.MapGet("/", async (TipoOportunidade? tipo, OportunidadeService service, CancellationToken ct) =>
                TypedResults.Ok(await service.ListAsync(tipo, ct)))
            .WithName("ListarOportunidades")
            .WithSummary("Lista as oportunidades (crédito, seguro, programas e incentivos)")
            .WithDescription("Filtro opcional: ?tipo=CreditoRural, Seguro, ProgramaGoverno ou Incentivo.")
            .Produces<IReadOnlyList<OportunidadeResponse>>()
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id}", async (int id, OportunidadeService service, CancellationToken ct) =>
                TypedResults.Ok(await service.GetAsync(id, ct)))
            .WithName("ObterOportunidade")
            .WithSummary("Detalha uma oportunidade, com os documentos exigidos")
            .Produces<OportunidadeResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
