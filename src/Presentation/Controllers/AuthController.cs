using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (registerDto.Password != registerDto.ConfirmPassword)
                return BadRequest("Passwords do not match");
            
            await _authService.Register(registerDto);
            
            return Ok("User registered successfully");
            }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var token = await _authService.Login(loginDto);
        return Ok(new { Token = token });
    }


    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] changePasswordDto changePasswordDto)
    {
        if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
            return BadRequest("New passwords do not match");
        
        var userId = User.Claims.FirstOrDefault(c => c.Type == "userId" || c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("User not authenticated");

        await _authService.ChangePassword(changePasswordDto, userId);
        
        return Ok("Password changed successfully");
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        await _authService.ResetPassword(resetPasswordDto);
        return Ok("Password reset successfully");
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        await _authService.ForgotPassword(email);
        return Ok("If an account with that email exists, a password reset link has been sent.");
    }

    [HttpDelete("delete-user")]
    public async Task<IActionResult> DeleteUser([FromBody] string email)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "userId" || c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("User not authenticated");

        await _authService.DeleteUser(email, userId);
        return Ok("User deleted successfully");
    }
}