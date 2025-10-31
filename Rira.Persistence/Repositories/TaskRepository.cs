using Microsoft.EntityFrameworkCore;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;
using Rira.Persistence.Data;

namespace Rira.Persistence.Repositories
{
    /// <summary>
    /// 🧩 [RiraDocs]
    /// =======================================================
    /// ریپازیتوری EF Core برای جدول Tasks مطابق اصول Clean Architecture
    /// -------------------------------------------------------
    /// هدف: مدیریت CRUD برای TaskEntity با پشتیبانی از حذف نرم (Soft Delete)
    /// </summary>
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        // ===========================================
        // 📄 دریافت همه‌ی تسک‌ها (به جز حذف‌شده‌ها)
        // ===========================================
        public async Task<List<TaskEntity>> GetAllAsync()
        {
            return await _context.Tasks
                .Where(t => !t.IsDeleted)
                .ToListAsync();
        }

        // ===========================================
        // 🔍 دریافت تسک بر اساس آیدی
        // ===========================================
        public async Task<TaskEntity?> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        // ===========================================
        // 🟩 ایجاد تسک جدید
        // خروجی: شناسه (ID) رکورد افزوده‌شده
        // ===========================================
        public async Task<int> CreateAsync(TaskEntity entity)
        {
            _context.Tasks.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        // ===========================================
        // ✳️ بروزرسانی تسک موجود
        // خروجی: 1 اگر موفقیت‌آمیز، 0 در غیر این صورت
        // ===========================================
        public async Task<int> UpdateAsync(TaskEntity entity)
        {
            var existing = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == entity.Id && !t.IsDeleted);

            if (existing == null)
                return 0;

            _context.Entry(existing).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return 1;
        }

        // ===========================================
        // 🗑 حذف نرم (Soft Delete)
        // خروجی: true اگر موفقیت‌آمیز
        // ===========================================
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
