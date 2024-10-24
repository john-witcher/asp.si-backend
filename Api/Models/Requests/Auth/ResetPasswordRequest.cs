using System.ComponentModel.DataAnnotations;

namespace Api.Models.Requests.Auth;

public class ResetPasswordRequest
{
    [Required, EmailAddress]
    public required string Email {get;set;}
    [Required]
    public required string Token {get;set;}
    [Required, DataType(DataType.Password)]
    public required string Password {get;set;}
    [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public required string ConfirmPassword {get;set;}
        
}