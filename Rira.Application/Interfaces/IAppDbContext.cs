using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Rira.Domain.Entities;

namespace Rira.Application.Interfaces
{
    /// <summary>
    /// رابط عمومی EF Core برای کار با داده‌ها در پروژه ریرا.
    /// نسخهٔ هماهنگ‌شده با EF Core 8 و تست‌های Mock.
    /// </summary>
    public interface IAppDbContext
    {
        // 🟢 DbSet‌های اصلی دامنه
        DbSet<TaskEntity> Tasks { get; set; }
        DbSet<EmployeeEntity> Employees { get; set; }

        // ✅ متد استاندارد EF برای دسترسی جنریک به DbSet‌ها
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        // ✅ ذخیره‌سازی تغییرات
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
