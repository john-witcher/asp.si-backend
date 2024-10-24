namespace Api.Exceptions.Cars;


public sealed class CarByIdNotFoundException(Guid id)
    : NotFoundException($"Car with id '{id}' doesn't exist in the database.");

public sealed class CarByNameNotFoundException(string name)
    : NotFoundException($"Car with name '{name}' doesn't exist in the database.");
