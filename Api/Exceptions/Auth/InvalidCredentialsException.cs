namespace Api.Exceptions.Auth;

public sealed class InvalidCredentialsException()
    : UnauthorizedException($"Invalid login attempt. Please check your credentials and try again.");