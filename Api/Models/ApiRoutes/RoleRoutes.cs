namespace Api.Models.ApiRoutes;

public static class RoleRoutes
{
    private const string BaseUrl = $"{BaseRoutes.BaseUrl}/{BaseRoutes.RoleController}";

    public const string GetAllRolesUrl = BaseUrl;
    public static string GetRoleByNameUrl(string roleName) => $"{BaseUrl}/{roleName}";
    public const string AddRoleUrl = BaseUrl;
    public static string RemoveRoleUrl(string roleName) => $"{BaseUrl}/{roleName}";
}