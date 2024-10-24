namespace Api.Exceptions.Auth;

public sealed class CantDeleteRoleException : PreconditionFailedException
{
    public CantDeleteRoleException(string roleName)
        :base ($"The role '{roleName}' can not be deleted, because it still has users associated with it.")
    { }
}