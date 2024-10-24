namespace Api.Exceptions.Users;

public sealed class UserByEmailAlreadyExistsException(string email)
    : AlreadyExistsException($"User with email '{email}' already exists in the database");
    
public sealed class UserByNameAlreadyExistsException(string username)
    : AlreadyExistsException($"User with username '{username}' already exists in the database");