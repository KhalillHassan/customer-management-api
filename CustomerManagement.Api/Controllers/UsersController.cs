using Asp.Versioning;
using CustomerManagement.Business.DTOs.Users;
using CustomerManagement.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{

    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
    CreateUserRequest request)
    {
        await _userService.CreateAsync(request);

        return StatusCode(
            StatusCodes.Status201Created);
    }
}