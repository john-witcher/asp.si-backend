namespace Api.Models.ApiRoutes;

public static class UserRoutes
{
    private const string BaseUrl = $"{BaseRoutes.BaseUrl}/{BaseRoutes.UserController}";
        
    public const string GetAllUsersUrl = BaseUrl;
    public static string GetUserByEmailUrl(string email) => $"{BaseUrl}/{email}";
    public static string AddUserUrl() => BaseUrl;
    public static string RemoveUserUrl(string userId) => $"{BaseUrl}/{userId}";
    public static string UpdateUserUrl(string userId) => $"{BaseUrl}/{userId}";
}