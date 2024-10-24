namespace Api.Exceptions.Auth;

public sealed class RoleNotFoundException(string roleName)
    : NotFoundException($"Role {roleName} doesn't exist in the database.");