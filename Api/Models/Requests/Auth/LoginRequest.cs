using System.ComponentModel.DataAnnotations;

namespace Api.Models.Requests.Auth;

public class LoginRequest
{
    [Required]
    public required string Username {get; init; }
    [Required, DataType(DataType.Password)]
    public required string Password {get; init; }
}