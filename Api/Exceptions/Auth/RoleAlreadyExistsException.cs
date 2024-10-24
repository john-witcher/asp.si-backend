namespace Api.Exceptions.Auth;

public sealed class RoleAlreadyExistsException(string roleName)
    : AlreadyExistsException($"Role {roleName} already exists in the database");