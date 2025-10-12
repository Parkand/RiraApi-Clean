using Microsoft.EntityFrameworkCore;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;
using System.Reflection;

namespace Rira.Persistence.Data
{
    /// <summary>
    /// 🌐 کلاس اصلی DbContext ریرا:
    /// وظیفه‌ی ارتباط بین دامنه (Entities) و دیتابیس را برعهده دارد.
    /// پیاده‌سازی <see cref="IAppDbContext"/> برای تزریق انتزاعی در Handlerها.
    /// </summary>
    public class AppDbContext : DbContext, IAppDbContext
    {
        /// <summary>
        /// سازنده‌ی اصلی که به‌صورت تزریق وابستگی (DI) فراخوانی می‌شود.
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// مجموعه‌ی جدول کارمندان (Employees Table)
        /// </summary>
        public DbSet<EmployeeEntity> Employees { get; set; }

        /// <summary>
        /// مجموعه‌ی جدول وظایف یا Taskها (Tasks Table)
        /// </summary>
        public DbSet<TaskEntity> Tasks { get; set; }

        /// <summary>
        /// ذخیره‌سازی تغییرات در پایگاه داده با پشتیبانی از CancellationToken.
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// اعمال تمامی پیکربندی‌های Fluent از اسمبلی جاری (برای Entities و Seed Data).
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // بارگذاری تنظیمات Fluent از اسمبلیِ پروژه‌ی Persistence
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
