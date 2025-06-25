using EFDataAccessLibrary.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RoadBlockTracker.Services.Interfaces;
using System.Security.Claims;

namespace RoadBlockTracker.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request.Email, request.Password);
            if (!result.Success) return Unauthorized(result.Message);

            SetRefreshTokenCookie(result.RefreshToken!);
            return Ok(new { result.AccessToken, result.ExpiresIn });
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken)) return Unauthorized();

            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (!result.Success) return Unauthorized(result.Message);

            SetRefreshTokenCookie(result.RefreshToken!);
            return Ok(new { result.AccessToken, result.ExpiresIn });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _authService.LogoutAsync(userId);

            Response.Cookies.Delete("refreshToken");
            return NoContent();
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(
                    _configuration.GetValue<int>("Jwt:RefreshTokenExpiryInDays")),
                Secure = true,
                SameSite = SameSiteMode.Strict
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] EFDataAccessLibrary.Models.Auth.RegisterRequest request)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/api/auth/confirm-email";
            var result = await _authService.RegisterAsync(request, baseUrl);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] int userId)
        {
            var result = await _authService.ConfirmEmailAsync(userId);

            if (!result)
                return NotFound("User not found or already confirmed.");

            return Ok("Email confirmed successfully.");
        }


    }
}
