using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs;
using TaskManagement.API.Models;
using TaskManagement.API.Repositories;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null)
                return NotFound(new { message = $"User with ID {id} not found." });
            return Ok(MapToDto(user));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.FullName))
                return BadRequest(new { message = "Username, password, and full name are required." });

            if (await _userRepository.UsernameExistsAsync(dto.Username))
                return Conflict(new { message = $"Username '{dto.Username}' is already taken." });

            var user = new User
            {
                Username = dto.Username.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName.Trim(),
                Email = dto.Email?.Trim()
            };

            var created = await _userRepository.CreateAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.FullName))
                return BadRequest(new { message = "Full name is required." });

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email?.Trim()
            };

            var updated = await _userRepository.UpdateAsync(id, user);
            if (updated is null)
                return NotFound(new { message = $"User with ID {id} not found." });

            return Ok(MapToDto(updated));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"User with ID {id} not found." });

            return NoContent();
        }

        private static UserDto MapToDto(User user) => new()
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Email = user.Email
        };
    }
}
