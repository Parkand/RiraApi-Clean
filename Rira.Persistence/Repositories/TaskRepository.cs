using Microsoft.EntityFrameworkCore;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;
using Rira.Persistence.Data;
namespace Rira.Persistence.Repositories
{
    /// <summary>
    /// پیاده‌سازی ریپازیتوری با EF Core برای جدول Tasks
    /// </summary>
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskEntity>> GetAllAsync()
        {
            return await _context.Tasks.Where(t => !t.IsDeleted).ToListAsync();
        }

        public async Task<TaskEntity?> GetByIdAsync(int id)
        {
            return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<TaskEntity> CreateAsync(TaskEntity entity)
        {
            _context.Tasks.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(TaskEntity entity)
        {
            var existing = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == entity.Id);
            if (existing == null) return false;

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return false;

            task.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
