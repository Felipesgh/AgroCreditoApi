namespace AgroCredito.Application.Exceptions;

/// <summary>Registro duplicado (ex.: e-mail já cadastrado). A API converte em HTTP 409.</summary>
public class ConflictException(string message) : Exception(message);

/// <summary>Login recusado. A mensagem é a mesma para e-mail inexistente e senha errada. A API converte em HTTP 401.</summary>
public class InvalidCredentialsException() : Exception("E-mail ou senha inválidos.");
