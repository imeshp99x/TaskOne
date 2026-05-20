using TaskManagement.API.Enums;

namespace TaskManagement.API.DTOs
{
    public class TaskItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public WorkTaskStatus Status { get; set; }
        public string StatusLabel { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public int? AssigneeId { get; set; }
        public string? AssigneeName { get; set; }
    }

    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public WorkTaskStatus Status { get; set; } = WorkTaskStatus.New;
        public DateTime? DueDate { get; set; }
        public int? AssigneeId { get; set; }
    }

    public class UpdateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public WorkTaskStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
        public int? AssigneeId { get; set; }
    }
}
