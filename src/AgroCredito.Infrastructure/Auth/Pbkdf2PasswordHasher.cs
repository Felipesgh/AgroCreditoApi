using System.Security.Cryptography;
using AgroCredito.Application.Abstractions;

namespace AgroCredito.Infrastructure.Auth;

/// <summary>
/// Hash de senha com PBKDF2-SHA256 e sal aleatório por senha.
/// Formato guardado: v1.{iterações}.{sal}.{hash} (tudo em Base64).
/// </summary>
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 600_000;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
        return $"v1.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public bool Verify(string password, string hash)
    {
        var partes = hash.Split('.');
        if (partes.Length != 4 || partes[0] != "v1" || !int.TryParse(partes[1], out var iteracoes) || iteracoes <= 0)
            return false;

        try
        {
            var salt = Convert.FromBase64String(partes[2]);
            var esperado = Convert.FromBase64String(partes[3]);
            var atual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iteracoes, HashAlgorithmName.SHA256, esperado.Length);
            return CryptographicOperations.FixedTimeEquals(atual, esperado);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
