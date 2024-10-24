using Api.Models;
using Api.Models.ApiRoutes;
using Api.Models.Requests.Users;
using Api.Services;
using Api.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
// [Authorize(Policy = AuthPolicy.Admin)]
[Route(BaseRoutes.UserController)]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UserService _userService;

    public UserController(ILogger<UserController> logger, UserService userService)
    {
        _logger = logger;
        _userService = userService;

        _logger.LogDebug("UserController instantiated.");
    }
        
        
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _userService.GetAllUsersAsync();

        return Ok(result);
    }
    
    [HttpGet("{email}")]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        var result = await _userService.GetUserByUsername(email);

        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] AddUserRequest request)
    {
        var result = await _userService.AddUserUserAsync(request);

        return Ok(result);
    }
    
    [HttpDelete("{userEmail}")]
    public async Task<IActionResult> RemoveUser(string userEmail)
    {
        var result = await _userService.RemoveUserAsync(userEmail);
    
        return Ok(result);
    }   
}