using System.ComponentModel.DataAnnotations;
using GiveMap_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiveMap_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var user = await _userService.RegisterUserAsync(model.Email, model.Password);
            _logger.LogInformation($"User registered: {model.Email}");
            return Ok(new { message = "User registered successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error registering user: {model.Email}");
            return StatusCode(500, new { message = "An error occurred while registering the user" });
        }
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult AdminOnly()
    {
        return Ok(new { message = "Admin access granted" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var token = await _userService.AuthenticateAsync(model.Email, model.Password);
            if (token == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(new { token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during login: {model.Email}");
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _userService.RequestPasswordResetAsync(model.Email);
            if (result)
            {
                return Ok(new { message = "Password reset email sent" });
            }

            return BadRequest(new { message = "Unable to process the request" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during password reset request: {model.Email}");
            return StatusCode(500, new { message = "An error occurred during the password reset request" });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _userService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);
            if (result)
            {
                return Ok(new { message = "Password reset successful" });
            }

            return BadRequest(new { message = "Invalid or expired password reset token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during password reset: {model.Email}");
            return StatusCode(500, new { message = "An error occurred during the password reset" });
        }
    }
}

public class RegisterModel
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required] [MinLength(8)] public string Password { get; set; }
}

public class LoginModel
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required] public string Password { get; set; }
}

public class ForgotPasswordModel
{
    [Required] [EmailAddress] public string Email { get; set; }
}

public class ResetPasswordModel
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required] public string Token { get; set; }

    [Required] [MinLength(8)] public string NewPassword { get; set; }
}