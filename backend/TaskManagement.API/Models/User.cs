using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Email { get; set; }

        public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
    }
}
