using AgroCredito.Application.DTOs;
using AgroCredito.Application.Services;

namespace AgroCredito.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Autenticação");

        group.MapPost("/register", async (RegisterRequest request, AuthService auth, CancellationToken ct) =>
            {
                var produtor = await auth.RegisterAsync(request, ct);
                return TypedResults.Created("/api/produtores/me", produtor);
            })
            .WithName("RegistrarProdutor")
            .WithSummary("Cadastra um novo produtor")
            .Produces<ProdutorResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/login", async (LoginRequest request, AuthService auth, CancellationToken ct) =>
                TypedResults.Ok(await auth.LoginAsync(request, ct)))
            .WithName("Login")
            .WithSummary("Autentica o produtor e devolve o token JWT")
            .Produces<TokenResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }
}
