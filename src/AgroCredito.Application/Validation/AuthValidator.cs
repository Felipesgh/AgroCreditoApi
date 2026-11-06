using System.Net.Mail;
using AgroCredito.Application.DTOs;

namespace AgroCredito.Application.Validation;

internal static class AuthValidator
{
    public static void ValidarCadastro(RegisterRequest r)
    {
        var erros = new ErrorCollector();

        if (string.IsNullOrWhiteSpace(r.Nome))
            erros.Add(nameof(r.Nome), "O nome é obrigatório.");
        else if (r.Nome.Trim().Length > 150)
            erros.Add(nameof(r.Nome), "O nome deve ter no máximo 150 caracteres.");

        var email = r.Email?.Trim();
        if (string.IsNullOrEmpty(email))
            erros.Add(nameof(r.Email), "O e-mail é obrigatório.");
        else if (email.Length > 200 || !MailAddress.TryCreate(email, out var endereco) || endereco.Address != email)
            erros.Add(nameof(r.Email), "Informe um e-mail válido.");

        if (string.IsNullOrEmpty(r.Senha))
            erros.Add(nameof(r.Senha), "A senha é obrigatória.");
        else if (r.Senha.Length < 8 || r.Senha.Length > 100)
            erros.Add(nameof(r.Senha), "A senha deve ter entre 8 e 100 caracteres.");
        else if (!r.Senha.Any(char.IsLetter) || !r.Senha.Any(char.IsDigit))
            erros.Add(nameof(r.Senha), "A senha deve conter letras e números.");

        erros.ThrowIfAny();
    }

    public static void ValidarLogin(LoginRequest r)
    {
        var erros = new ErrorCollector();

        if (string.IsNullOrWhiteSpace(r.Email))
            erros.Add(nameof(r.Email), "O e-mail é obrigatório.");

        if (string.IsNullOrEmpty(r.Senha))
            erros.Add(nameof(r.Senha), "A senha é obrigatória.");

        erros.ThrowIfAny();
    }
}
