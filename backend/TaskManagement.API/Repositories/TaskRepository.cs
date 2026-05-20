using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.Enums;
using TaskManagement.API.Models;

namespace TaskManagement.API.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync(
            WorkTaskStatus? statusFilter,
            int? assigneeIdFilter,
            string? sortBy,
            bool sortDesc)
        {
            IQueryable<TaskItem> query = _context.Tasks.Include(t => t.Assignee);

            if (statusFilter.HasValue)
                query = query.Where(t => t.Status == statusFilter.Value);

            if (assigneeIdFilter.HasValue)
                query = query.Where(t => t.AssigneeId == assigneeIdFilter.Value);

            query = (sortBy?.ToLower(), sortDesc) switch
            {
                ("title", false) => query.OrderBy(t => t.Title),
                ("title", true) => query.OrderByDescending(t => t.Title),
                ("status", false) => query.OrderBy(t => t.Status),
                ("status", true) => query.OrderByDescending(t => t.Status),
                ("duedate", false) => query.OrderBy(t => t.DueDate),
                ("duedate", true) => query.OrderByDescending(t => t.DueDate),
                ("assignee", false) => query.OrderBy(t => t.Assignee!.FullName),
                ("assignee", true) => query.OrderByDescending(t => t.Assignee!.FullName),
                _ => query.OrderByDescending(t => t.CreatedAt)
            };

            return await query.ToListAsync();
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.Tasks.Include(t => t.Assignee).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            task.CreatedAt = DateTime.UtcNow;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return (await GetByIdAsync(task.Id))!;
        }

        public async Task<TaskItem?> UpdateAsync(int id, TaskItem updatedTask)
        {
            var existing = await _context.Tasks.FindAsync(id);
            if (existing is null) return null;

            existing.Title = updatedTask.Title;
            existing.Description = updatedTask.Description;
            existing.Status = updatedTask.Status;
            existing.DueDate = updatedTask.DueDate;
            existing.AssigneeId = updatedTask.AssigneeId;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (await GetByIdAsync(id))!;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task is null) return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
