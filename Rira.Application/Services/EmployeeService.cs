using Microsoft.EntityFrameworkCore;
using Rira.Application.Interfaces;

namespace Rira.Application.Services
{
    // 🧩 سرویس مدیریتی کارمندان (EmployeeService)
    // --------------------------------------------------------------------
    // این کلاس در لایه‌ی Application قرار گرفته و منطق سطح میانی (Service Layer)
    // مربوط به بررسی داده‌های تکراری یا کمکی برای Handlerها را پیاده‌سازی می‌کند.
    // 
    // 🎯 هدف اصلی:
    //     ▫ تجرید و جداسازی منطق تکراری بررسی داده (Validation Helpers)
    //       از Handlerها و Validatorها.
    //     ▫ ایجاد قابلیت تست واحد آسان از طریق تزریق وابستگی (Dependency Injection).
    //
    // ⚙️ ارتباطات:
    //     ▫ وابسته به IAppDbContext ← برای دسترسی به EF Core.
    //     ▫ پیاده‌کننده‌ی IEmployeeService ← قرارداد بین Handler و سرویس.
    //
    // 🔹 نکته طراحی RiRaDocs:
    //     Serviceها در RiRa.Application نقش «لایه‌ی میانی» میان Handler و DbContext را دارند؛
    //     بنابراین هیچ منطق نمایشی یا کنترلری در این سطح نباید وجود داشته باشد.
    public class EmployeeService : IEmployeeService
    {
        // ============================================================
        // 🧱 وابستگی به DbContext از طریق اینترفیس IAppDbContext
        // ============================================================
        // در معماری تمیز (Clean Architecture) نباید مستقیماً به کلاس AppDbContext
        // ارجاع داده شود. به همین خاطر، تزریق از نوع IAppDbContext انجام می‌شود.
        private readonly IAppDbContext _context;

        // ============================================================
        // 🧩 سازنده کلاس - تزریق وابستگی (Dependency Injection)
        // ============================================================
        // سرویس در DI Container (در Program.cs) ثبت شده است.
        // زمان اجرا، EF Core DbContext واقعی جایگزین IAppDbContext می‌شود.
        public EmployeeService(IAppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // ✅ بررسی تکراری بودن ایمیل کارمند
        // ============================================================
        // این متد بررسی می‌کند آیا ایمیل وارد‌شده در پایگاه داده موجود است یا خیر.
        // 
        // 🧠 منطق:
        //     از متد EF Core → AnyAsync استفاده می‌شود تا سریع‌ترین بررسی وجودی انجام شود.
        // ⚙️ مثال استفاده در Handler:
        //     if (await _employeeService.IsEmailDuplicateAsync(command.Email))
        //         throw new BusinessException("ایمیل تکراری است.");
        public async Task<bool> IsEmailDuplicateAsync(string email)
        {
            return await _context.Employees.AnyAsync(e => e.Email == email);
        }

        // ============================================================
        // ✅ بررسی تکراری بودن شماره موبایل کارمند
        // ============================================================
        // مشابه متد ایمیل، شماره تلفن کارمند را برای تکراری بودن بررسی می‌کند.
        //
        // 🎓 نکته آموزشی:
        //     برای کاهش بار دیتابیس، بهتر است این گونه متدها همیشه به شکل Async صدا زده شوند،
        //     به‌ویژه در Commandهای ثبت یا بروزرسانی.
        // ⚙️ مثال استفاده در Validator:
        //     RuleFor(x => x.MobileNumber)
        //         .MustAsync(async (mobile, ct) => !await _employeeService.IsMobileDuplicateAsync(mobile))
        //         .WithMessage("شماره موبایل تکراری است.");
        public async Task<bool> IsMobileDuplicateAsync(string mobileNumber)
        {
            return await _context.Employees.AnyAsync(e => e.MobileNumber == mobileNumber);
        }
    }

    // ===========================================================================================
    // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
    // -------------------------------------------------------------------------------------------
    // 🔹 مفهوم: EmployeeService منطق کمکی و بررسی داده‌های تکراری برای کارمندان را
    //           در لایه‌ی Application ارائه می‌دهد.
    // 🔹 اهداف کلیدی:
    //     ▫ جداسازی Validation از Handlerها.
    //     ▫ تسهیل Dependency Injection و تست‌پذیری.
    //     ▫ ارتباط مستقیم EF Core از طریق IAppDbContext, نه کلاس واقعی.
    //
    // 🔹 کاربرد در CQRS:
    //     ▫ Commandهای Create/Update از این سرویس برای بررسی تکراری بودن ایمیل یا موبایل استفاده می‌کنند.
    //
    // 🔹 اصول معماری رعایت‌شده:
    //     ▫ Separation of Concerns — منطق بررسی داده از منطق تجاری جداست.
    //     ▫ Clean Architecture — ارتباط با DbContext فقط از طریق اینترفیس.
    //     ▫ Async Programming Best Practice — تمام عملیات پایگاه داده ناهم‌زمان اجرا می‌شوند.
    //
    // 🔹 تگ انتشار RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===========================================================================================
}
