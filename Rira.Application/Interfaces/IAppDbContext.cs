using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Rira.Domain.Entities;

namespace Rira.Application.Interfaces
{
    // 🧩 رابط عمومی EF Core برای کار با داده‌ها در پروژه ریرا — IAppDbContext
    // -------------------------------------------------------------------
    // این Interface یکی از ستون‌های اصلی لایه‌ی Application در معماری تمیز (Clean Architecture) است.
    //
    // 📌 هدف:
    //   ▫ جداسازی وابستگی مستقیم به کلاس DbContext واقعی از سایر بخش‌ها (مثل Handlerها و Commandها).
    //   ▫ امکان تست‌پذیری (Mock/Fake Context) بدون نیاز به دیتابیس واقعی.
    //   ▫ فراهم‌سازی نقطه‌ی اشتراک بین Data Access Layer و Application Layer.
    //
    // ⚙️ نسخه‌ی هماهنگ‌شده با EF Core 8 و تکنیک‌های Unit Testing در پروژه RiRa.
    //
    // 🔹 این Interface معمولاً در فایل Startup یا Program.cs توسط
    //     services.AddScoped<IAppDbContext, AppDbContext>();
    //
    //     متصل می‌شود تا DI (Dependency Injection) به‌درستی کار کند.
    public interface IAppDbContext
    {
        // 🟢 DbSet‌های اصلی دامنه
        // -------------------------------------------------------------------
        // هر DbSet معرف یک جدول در پایگاه داده است.
        // برای دسترسی به داده‌های Domain Entityها در قسمت Application استفاده می‌شود.

        /// <summary>
        /// مجموعه‌ی اصلی تسک‌ها در سیستم (Tasks)
        /// </summary>
        DbSet<TaskEntity> Tasks { get; set; }

        /// <summary>
        /// مجموعه‌ی کارمندان در سیستم (Employees)
        /// </summary>
        DbSet<EmployeeEntity> Employees { get; set; }

        // ✅ متد استاندارد EF برای دسترسی جنریک به DbSet‌ها
        // -------------------------------------------------------------------
        // این متد به شما امکان می‌دهد هر DbSet دلخواه را بدون دانستن نام آن واکشی کنید.
        // برای عملیات جنریک یا الگوهای Repository بسیار مفید است.
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        // ✅ ذخیره‌سازی تغییرات در سطح DbContext
        // -------------------------------------------------------------------
        // این متد برای ثبت تغییرات در دیتابیس استفاده می‌شود
        // و همان نقش SaveChangesAsync استاندارد EF Core را دارد.
        // در هندلرهای Command (مثل Create, Update, Delete) از آن استفاده می‌شود.
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    // ===========================================================================================
    // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
    // -------------------------------------------------------------------------------------------
    // 🔹 نقش IAppDbContext در معماری تمیز:
    //     ▫ به‌جای آن‌که Handlerها به DbContext واقعی وابسته باشند، به این Interface متصل می‌شوند.
    //     ▫ این باعث می‌شود Application کاملاً از EF Core جدا و تست‌پذیر بماند.
    //
    // 🔹 کاربرد در لایه‌های مختلف:
    //     ▫ Application → وابسته به Interface (IAppDbContext)
    //     ▫ Infrastructure → پیاده‌سازی کلاس واقعی (AppDbContext)
    //     ▫ Tests → استفاده از نسخه‌های Mock برای سناریوهای تستی.
    //
    // 🔹 مزیت اصلی:
    //     ▫ جداسازی وابستگی‌ها → سادگی در Mock و تزریق در تست‌ها.
    //     ▫ انعطاف‌پذیری در مهاجرت داده‌ای، بدون وابستگی به EF Core واقعی.
    //
    // 🔹 نکتهٔ آموزشی EF Core 8:
    //     ▫ نسخه‌ی جدید EF، عملیات بهینه‌تر برای AsNoTracking و ترنزکشن را بهتر پشتیبانی می‌کند.
    //
    // 🔹 تگ انتشار:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===========================================================================================
}
