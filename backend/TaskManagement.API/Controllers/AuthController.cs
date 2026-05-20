using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using TaskManagement.API.DTOs;
using TaskManagement.API.Repositories;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Username and password are required." });

            var user = await _userRepository.GetByUsernameAsync(dto.Username.Trim());
            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid username or password." });

            return Ok(new LoginResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName
            });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var username = User.Identity?.Name;
            var fullName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;

            return Ok(new LoginResponseDto
            {
                Id = int.TryParse(id, out var parsedId) ? parsedId : 0,
                Username = username ?? string.Empty,
                FullName = fullName ?? string.Empty
            });
        }
    }
}
