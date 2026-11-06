using AgroCredito.Application.Exceptions;
using AgroCredito.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AgroCredito.Api.Errors;

/// <summary>Converte as exceções da aplicação em respostas HTTP padronizadas (ProblemDetails).</summary>
public sealed class ApiExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, titulo) = exception switch
        {
            ValidationException => (StatusCodes.Status400BadRequest, "Dados inválidos"),
            InvalidCredentialsException => (StatusCodes.Status401Unauthorized, "Credenciais inválidas"),
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflito"),
            DomainException => (StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada"),
            _ => (0, string.Empty)
        };

        if (status == 0)
            return false; // erro inesperado: o ASP.NET responde 500

        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = exception.Message
        };

        if (exception is ValidationException validacao)
            problema.Extensions["errors"] = validacao.Errors;

        httpContext.Response.StatusCode = status;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problema,
            Exception = exception
        });
    }
}
