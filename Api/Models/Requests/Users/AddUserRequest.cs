using System.ComponentModel.DataAnnotations;

namespace Api.Models.Requests.Users;

public class AddUserRequest
{
    [Required]
    public required string Username {get;set;}
    [Required, DataType(DataType.Password)]
    public required string Password {get;set;}
    [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public required string ConfirmPassword {get;set;}
}