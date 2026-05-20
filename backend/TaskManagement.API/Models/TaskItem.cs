using System.ComponentModel.DataAnnotations;
using TaskManagement.API.Enums;

namespace TaskManagement.API.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public WorkTaskStatus Status { get; set; } = WorkTaskStatus.New;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DueDate { get; set; }

        public int? AssigneeId { get; set; }
        public User? Assignee { get; set; }
    }
}
