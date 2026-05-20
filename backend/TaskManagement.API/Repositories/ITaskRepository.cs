using TaskManagement.API.Enums;
using TaskManagement.API.Models;

namespace TaskManagement.API.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TaskItem>> GetAllAsync(WorkTaskStatus? statusFilter, int? assigneeIdFilter, string? sortBy, bool sortDesc);
        Task<TaskItem?> GetByIdAsync(int id);
        Task<TaskItem> CreateAsync(TaskItem task);
        Task<TaskItem?> UpdateAsync(int id, TaskItem task);
        Task<bool> DeleteAsync(int id);
    }
}
