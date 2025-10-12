using Microsoft.EntityFrameworkCore;
using Rira.Domain.Entities;
using Rira.Application.Interfaces;

namespace Rira.Persistence.Data
{
    /// <summary>
    /// 🧩 کلاس اصلی اتصال به پایگاه داده در پروژه ریرا
    /// این کلاس DbContext را پیاده‌سازی کرده و از IAppDbContext در لایه Application استفاده می‌کند.
    /// </summary>
    public class AppDbContext : DbContext, IAppDbContext
    {
        /// <summary>
        /// سازنده‌ی کانتکست با دریافت تنظیمات اتصال از DI
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// جدول وظایف (تسک‌ها)
        /// </summary>
        public DbSet<TaskEntity> Tasks { get; set; }

        /// <summary>
        /// تنظیمات مدل‌سازی و فیلدهای ویژه (در زمان مایگریشن)
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تعریف کلید اصلی برای جدول وظایف
            modelBuilder.Entity<TaskEntity>()
                .HasKey(t => t.Id);

            // تعریف شرط حذف نرم
            modelBuilder.Entity<TaskEntity>()
                .HasQueryFilter(t => !t.IsDeleted);

            // محدودسازی طول رشته‌های تاریخ برای سازگاری دیتابیس
            modelBuilder.Entity<TaskEntity>()
                .Property(t => t.CreatedAt)
                .HasMaxLength(10);

            modelBuilder.Entity<TaskEntity>()
                .Property(t => t.UpdatedAt)
                .HasMaxLength(10);
        }
    }
}
