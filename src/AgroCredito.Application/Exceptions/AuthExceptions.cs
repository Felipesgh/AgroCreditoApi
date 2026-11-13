namespace AgroCredito.Application.Exceptions;

public class ConflictException(string message) : Exception(message);

public class InvalidCredentialsException() : Exception("E-mail ou senha inválidos.");
