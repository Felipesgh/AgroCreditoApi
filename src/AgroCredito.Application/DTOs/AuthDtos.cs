using AgroCredito.Domain.Entities;

namespace AgroCredito.Application.DTOs;

public sealed record RegisterRequest(string Nome, string Email, string Senha);

public sealed record LoginRequest(string Email, string Senha);

public sealed record TokenResponse(string AccessToken, string TokenType, int ExpiresIn);

public sealed record ProdutorResponse(int Id, string Nome, string Email);

internal static class AuthMappings
{
    public static ProdutorResponse ToResponse(this Produtor p) => new(p.Id, p.Nome, p.Email);
}
