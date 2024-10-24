namespace Api.Exceptions.Users;

public sealed class UserByEmailNotFoundException(string userEmail)
    : NotFoundException($"User with email '{userEmail}' doesn't exist in the database.");
    
public sealed class UserByIdNotFoundException(Guid userId)
    : NotFoundException($"User with id '{userId}' doesn't exist in the database.");
    
public sealed class UserNameIdNotFoundException(string username)
    : NotFoundException($"User with username '{username}' doesn't exist in the database.");