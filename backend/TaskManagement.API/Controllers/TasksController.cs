using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.API.DTOs;
using TaskManagement.API.Enums;
using TaskManagement.API.Models;
using TaskManagement.API.Repositories;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;

        public TasksController(ITaskRepository taskRepository, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] WorkTaskStatus? status,
            [FromQuery] int? assigneeId,
            [FromQuery] string? sortBy,
            [FromQuery] bool sortDesc = false)
        {
            var tasks = await _taskRepository.GetAllAsync(status, assigneeId, sortBy, sortDesc);
            var dtos = tasks.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task is null)
                return NotFound(new { message = $"Task with ID {id} not found." });
            return Ok(MapToDto(task));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest(new { message = "Title is required." });

            if (dto.AssigneeId.HasValue)
            {
                var assignee = await _userRepository.GetByIdAsync(dto.AssigneeId.Value);
                if (assignee is null)
                    return BadRequest(new { message = $"Assignee with ID {dto.AssigneeId} not found." });
            }

            var task = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                Status = dto.Status,
                DueDate = dto.DueDate,
                AssigneeId = dto.AssigneeId
            };

            var created = await _taskRepository.CreateAsync(task);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest(new { message = "Title is required." });

            if (dto.AssigneeId.HasValue)
            {
                var assignee = await _userRepository.GetByIdAsync(dto.AssigneeId.Value);
                if (assignee is null)
                    return BadRequest(new { message = $"Assignee with ID {dto.AssigneeId} not found." });
            }

            var task = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                Status = dto.Status,
                DueDate = dto.DueDate,
                AssigneeId = dto.AssigneeId
            };

            var updated = await _taskRepository.UpdateAsync(id, task);
            if (updated is null)
                return NotFound(new { message = $"Task with ID {id} not found." });

            return Ok(MapToDto(updated));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _taskRepository.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Task with ID {id} not found." });

            return NoContent();
        }

        private static TaskItemDto MapToDto(TaskItem task) => new()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            StatusLabel = task.Status switch
            {
                WorkTaskStatus.New => "New",
                WorkTaskStatus.InDevelopment => "In Development",
                WorkTaskStatus.InTesting => "In Testing",
                WorkTaskStatus.InDeployment => "In Deployment",
                WorkTaskStatus.Completed => "Completed",
                _ => "Unknown"
            },
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            DueDate = task.DueDate,
            AssigneeId = task.AssigneeId,
            AssigneeName = task.Assignee?.FullName
        };
    }
}
