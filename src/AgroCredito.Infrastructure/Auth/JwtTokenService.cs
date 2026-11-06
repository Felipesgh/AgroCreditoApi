using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AgroCredito.Application.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace AgroCredito.Infrastructure.Auth;

public sealed class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "AgroCreditoApi";
    public string Audience { get; set; } = "AgroCreditoApi";
    public int ExpirationMinutes { get; set; } = 60;
}

public sealed class JwtTokenService(JwtOptions options) : ITokenService
{
    public int ExpirationSeconds => options.ExpirationMinutes * 60;

    public string CreateToken(int produtorId, string nome, string email)
    {
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, produtorId.ToString()),
            new Claim("name", nome),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("role", "Produtor"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(options.ExpirationMinutes),
            signingCredentials: credenciais);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
