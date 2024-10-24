using System.ComponentModel.DataAnnotations;

namespace Api.Models.Requests.Auth;

public class RequestPasswordResetRequest
{
    [Required, EmailAddress]
    public required string Email {get;set;}
}