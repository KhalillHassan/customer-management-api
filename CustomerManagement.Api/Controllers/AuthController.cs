using Asp.Versioning;
using CustomerManagement.Business.DTOs.Auth;
using CustomerManagement.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authenticates a user and returns a JWT access token.
    /// </summary>
    /// <param name="request">
    /// The user's email address and password.
    /// </param>
    /// <returns>
    /// A JWT access token and its expiration date.
    /// </returns>
    [HttpPost("login")]
    [ProducesResponseType(
        typeof(LoginResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ValidationProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(string),
        StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var result =
            await _authService.LoginAsync(request);

        if (result is null)
        {
            return Unauthorized(
                "Invalid email or password.");
        }

        return Ok(result);
    }
}