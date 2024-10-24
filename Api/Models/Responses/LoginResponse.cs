

using Api.Models.Db;

namespace Api.Models.Responses;

public class LoginResponse
{
    public required string JwtToken { get; init; }
    public required DateTime JwtExpire { get; init; }
    public required string Email { get; init; }
    public required string Company { get; init; }
    public required GlobalSettings GlobalSettings { get; init; }
}