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
    /// <summary>
    /// Creates a new user account.
    /// </summary>
    /// <param name="request">The new user information.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(string),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        CreateUserRequest request)
    {
        try
        {
            await _userService.CreateAsync(request);

            return StatusCode(
                StatusCodes.Status201Created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}


