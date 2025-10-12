using Microsoft.EntityFrameworkCore;
using Rira.Domain.Entities;

namespace Rira.Application.Interfaces
{
    /// <summary>
    /// 🧩 واسط اصلی DbContext برای جداسازی وابستگی لایه Application از Persistence.
    /// تمام موجودیت‌های اصلی سیستم (Entities) در این Interface تعریف می‌شوند.
    /// </summary>
    public interface IAppDbContext
    {
        /// <summary>
        /// مجموعه‌ی کارمندان (Employees Table)
        /// </summary>
        DbSet<EmployeeEntity> Employees { get; set; }

        /// <summary>
        /// مجموعه‌ی وظایف یا تسک‌ها (Tasks Table)
        /// </summary>
        DbSet<TaskEntity> Tasks { get; set; }

        /// <summary>
        /// ثبت و ذخیره‌ی تغییرات در پایگاه داده به‌صورت ناهمزمان.
        /// </summary>
        /// <param name="cancellationToken">توکن لغو عملیات که در async استفاده می‌شود.</param>
        /// <returns>تعداد رکوردهای تغییر یافته.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
