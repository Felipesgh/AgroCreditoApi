using System.Security.Claims;
using AgroCredito.Application.Exceptions;

namespace AgroCredito.Api.Endpoints;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Lê o id do produtor logado a partir da claim "sub" do token.</summary>
    public static int GetProdutorId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirst("sub")?.Value;
        return int.TryParse(sub, out var id) ? id : throw new InvalidCredentialsException();
    }
}
