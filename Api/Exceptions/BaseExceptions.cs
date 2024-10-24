namespace Api.Exceptions;

public abstract class AlreadyExistsException(string message) : Exception(message);
public abstract class BadRequestException(string message) : Exception(message);
public abstract class NotFoundException(string message) : Exception(message);
public abstract class PreconditionFailedException(string message) : Exception(message);
public abstract class UnauthorizedException(string message) : Exception(message);

public sealed class DatabaseErrorException(string message, List<string>? errors = null) : Exception(message)
{
    public List<string>? Errors { get; set; } = errors;
}
