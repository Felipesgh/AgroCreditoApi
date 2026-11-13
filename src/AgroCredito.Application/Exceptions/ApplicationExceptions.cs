namespace AgroCredito.Application.Exceptions;

/// <summary>Recurso inexistente (ou de outro produtor). A API converte em HTTP 404.</summary>
public class NotFoundException(string message) : Exception(message);

/// <summary>Dados de entrada inválidos. A API converte em HTTP 400 com a lista de erros.</summary>
public class ValidationException(IReadOnlyDictionary<string, string[]> errors) : Exception("Um ou mais campos são inválidos.")
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
