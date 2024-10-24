namespace Api.Models.ApiRoutes;

public static class AuthRoutes
{
    private const string BaseUrl = $"{BaseRoutes.BaseUrl}/{BaseRoutes.AuthController}";

    public const string Login = "login";
    public const string Logout = "logout";
    public const string ConfirmEmail = "confirm-email";
    public const string ResendConfirmationEmail = "resend-confirmation-email";
    public const string RequestPasswordReset = "request-password-reset";
    public const string ResetPassword = "reset-password";
        
    public const string QueryEmail = "email";
    public const string QueryToken = "token";
        
    public const string LoginUrl = $"{BaseUrl}/{Login}";
    public const string LogoutUrl = $"{BaseUrl}/{Logout}";
    public static string ConfirmEmailUrl(string email, string token) => $"{BaseUrl}/{ConfirmEmail}?{QueryEmail}={email}&{QueryToken}={token}";
    public static string ResendConfirmationEmailUrl() => $"{BaseUrl}/{ResendConfirmationEmail}";
    public static string RequestPasswordResetUrl() => $"{BaseUrl}/{RequestPasswordReset}";
    public static string ResetPasswordUrl() => $"{BaseUrl}/{ResetPassword}";
}