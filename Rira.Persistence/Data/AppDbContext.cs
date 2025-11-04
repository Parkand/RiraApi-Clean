using Microsoft.EntityFrameworkCore;
using Rira.Domain.Entities;
using Rira.Application.Interfaces;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rira.Persistence.Data
{
    // 🧭 کلاس پایگاه داده اصلی پروژه (AppDbContext)
    // ===============================================================================
    // این کلاس نماینده‌ی «سشن ارتباطی» بین لایه‌ی Domain و دیتابیس واقعی است.
    // بر پایه EF Core 8 طراحی شده و از قرارداد IAppDbContext پیروی می‌کند.
    //
    // 🎯 هدف طراحی:
    //     ▫ نگهداری DbSetهای Entityهای اصلی (Task, Employee).
    //     ▫ اعمال تنظیمات Fluent API از Assembly فعلی.
    //     ▫ پشتیبانی از تست‌های واحد و Mocking از طریق IAppDbContext.
    //
    // 💡 نکته RiRaDocs:
    //     DbContext باید در لایه Persistence تعریف شود تا جداسازی وابستگی‌ها
    //     (Separation of Concerns) بین Domain و زیرساخت داده رعایت گردد.
    public class AppDbContext : DbContext, IAppDbContext
    {
        // -----------------------------------------------------------------------
        // 🏗️ سازنده‌ی کلاس — دریافت DbContextOptions از DI
        // -----------------------------------------------------------------------
        // در زمان اجرای ASP.NET Core، این سازنده توسط Dependency Injection
        // مقداردهی می‌شود. پارامتر options تعیین‌کننده‌ی نوع پایگاه داده و
        // تنظیمات مربوط به مبدأ اتصال (ConnectionString, Provider, LazyLoading و ...)
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // -----------------------------------------------------------------------
        // 🧩 تعریف DbSetها برای موجودیت‌های اصلی دامنه
        // -----------------------------------------------------------------------
        // DbSet<TaskEntity>  → جدول Tasks
        // DbSet<EmployeeEntity> → جدول Employees
        //
        // هر DbSet در EF به‌صورت خودکار به جدول هم‌نام (یا پیکربندی‌شده) نگاشت می‌شود.
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<EmployeeEntity> Employees { get; set; }

        // -----------------------------------------------------------------------
        // ⚙️ متد Set<TEntity>()
        // -----------------------------------------------------------------------
        // این Override از متد Set در DbContext به‌صورت صریح با new تعریف شده
        // تا از تداخل با پیاده‌سازی IAppDbContext جلوگیری شود (رفع خطای CS1061).
        // به وسیله‌ی این متد، هر DbSet عمومی را می‌توان به‌صورت جنریک از طریق
        // Interface فراخوانی کرد.
        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
            => base.Set<TEntity>();

        // -----------------------------------------------------------------------
        // 💾 متد ذخیره‌سازی تغییرات (Async)
        // -----------------------------------------------------------------------
        // نسخه غیرهمزمان از SaveChanges برای اطمینان از عملکرد بهینه در I/O.
        // هر فراخوانی EF در Handler یا Service باید از نسخه Async این متد استفاده کند.
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await base.SaveChangesAsync(cancellationToken);

        // -----------------------------------------------------------------------
        // 🧮 متد OnModelCreating — اعمال پیکربندی‌های Fluent API
        // -----------------------------------------------------------------------
        // در این متد، تنظیمات مربوط به هر Entity از طریق ApplyConfigurationsFromAssembly
        // به‌صورت خودکار بارگذاری می‌شوند.
        // تمامی کلاس‌های Configuration (مثل EmployeeConfiguration, TaskConfiguration)
        // باید در همین Assembly (لایه Persistence) قرار داشته باشند تا شناسایی شوند.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 📦 بارگذاری همه‌ی کلاس‌های IConfiguration دریافت‌شده از Assembly جاری.
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }

    // ===============================================================================
    // 📘 خلاصه آموزشی RiRaDocs Teaching Edition
    // ------------------------------------------------------------------------------
    // 🔹 نقش کلیدی در معماری تمیز:
    //     ▫ AppDbContext هسته‌ی ارتباط با پایگاه داده و نقطه تمایز لایه‌ی Persistence است.
    //     ▫ از طریق IAppDbContext تزریق می‌شود تا در تست‌ها و Mock Contextها استفاده گردد.
    //
    // 🔹 وابستگی‌ها:
    //     ▫ IAppDbContext (Application Layer Interface)
    //     ▫ EmployeeEntity & TaskEntity (Domain Layer)
    //     ▫ IConfiguration classes (Persistence.Configurations)
    //
    // 🔹 اصول طراحی رعایت‌شده:
    //     ▫ Dependency Inversion Principle (DIP)
    //     ▫ Testability via Interface Injection
    //     ▫ EF Core Fluent Configuration Discovery (Assembly Scanning)
    //
    // 🔹 نکات کاربردی:
    //     ▫ هنگام افزودن Entity جدید، تنها کافی‌ست Configuration مرتبط در این Assembly تعریف شود.
    //     ▫ نیازی به ثبت دستی DbSet در OnModelCreating نیست.
    //
    // 🔹 تگ انتشار RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===============================================================================
}
