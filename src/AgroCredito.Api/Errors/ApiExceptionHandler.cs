using AgroCredito.Application.Exceptions;
using AgroCredito.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AgroCredito.Api.Errors;

public sealed class ApiExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, titulo) = exception switch
        {
            BadHttpRequestException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            ValidationException => (StatusCodes.Status400BadRequest, "Dados inválidos"),
            InvalidCredentialsException => (StatusCodes.Status401Unauthorized, "Credenciais inválidas"),
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            ConflictException => (StatusCodes.Status409Conflict, "Conflito"),
            DomainException => (StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada"),
            _ => (0, string.Empty)
        };

        if (status == 0)
            return false; 

        var detalhe = exception is BadHttpRequestException
            ? "A requisição está mal formada (JSON inválido ou parâmetro com tipo incorreto)."
            : exception.Message;

        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = detalhe
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
