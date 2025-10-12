using Microsoft.EntityFrameworkCore;
using Rira.Domain.Entities;

namespace Rira.Application.Interfaces
{
    /// <summary>
    /// 🧩 واسطی از DbContext برای جداسازی وابستگی Application از Persistence
    /// </summary>
    public interface IAppDbContext
    {
        /// <summary>مجموعه‌ی وظایف</summary>
        DbSet<TaskEntity> Tasks { get; set; }

        /// <summary>ذخیره‌ی تغییرات در دیتابیس</summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
