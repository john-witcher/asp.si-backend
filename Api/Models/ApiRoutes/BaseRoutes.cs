namespace Api.Models.ApiRoutes;

public static class BaseRoutes
{
    public const string BaseUrl = "http://localhost:7296";
    private const string ApiVersion1 = "api/v1";
        
    public const string AuthController = $"{ApiVersion1}/authentication";
    public const string CompanyController = $"{ApiVersion1}/companies";
    public const string UserController = $"{ApiVersion1}/users";
    public const string RoleController = $"{ApiVersion1}/roles";
}