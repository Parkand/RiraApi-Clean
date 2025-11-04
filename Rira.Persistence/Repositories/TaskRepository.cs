using Microsoft.EntityFrameworkCore;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;
using Rira.Persistence.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rira.Persistence.Repositories
{
    // ⚙️ کلاس ریپازیتوری اصلی مربوط به TaskEntity
    // ===============================================================================
    // این کلاس پیاده‌سازی مستقیم ITaskRepository است و ارتباط بین لایه Application و
    // پایگاه داده را از طریق AppDbContext برقرار می‌کند.
    // مبنای کار با EF Core است و تمامی عملیات CRUD را با حفظ اصول Clean Architecture
    // انجام می‌دهد.
    //
    // 🎯 هدف RiRaDocs:
    //     ▫ جداسازی منطق داده از لایه سرویس (Dependency Inversion).
    //     ▫ پیاده‌سازی Soft Delete برای حفظ تاریخچه‌ی داده و جلوگیری از حذف فیزیکی.
    //     ▫ سازگاری کامل با تست‌های واحد (Mock Context).
    //
    // 🧩 نکته‌ی معماری:
    //     در معماری تمیز، Repository فقط با DbContext کار می‌کند و نباید منطق اعتبارسنجی
    //     یا نگاشت DTO را درون خود داشته باشد.
    public class TaskRepository : ITaskRepository
    {
        // -----------------------------------------------------------------------
        // 🧱 وابستگی به AppDbContext از طریق تزریق سازنده (Constructor Injection)
        // -----------------------------------------------------------------------
        // این شیء همان بستر ارتباط با پایگاه داده است و توسط DI Container برنامه
        // (مثلاً در Startup.cs یا Program.cs) تزریق می‌شود.
        private readonly AppDbContext _context;

        // 📦 سازنده کلاس
        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        // -----------------------------------------------------------------------
        // 📄 متد دریافت همه‌ی تسک‌ها
        // -----------------------------------------------------------------------
        // ❖ توضیح آموزشی RiRaDocs:
        //     - داده‌هایی که دارای IsDeleted=true هستند، فیلتر می‌شوند.
        //     - استفاده از LINQ و EF برای ترجمه مستقیم به SQL کوئری.
        // -----------------------------------------------------------------------
        public async Task<List<TaskEntity>> GetAllAsync()
        {
            return await _context.Tasks
                .Where(t => !t.IsDeleted)
                .ToListAsync();
        }

        // -----------------------------------------------------------------------
        // 🔍 متد دریافت تسک با شناسه (Id)
        // -----------------------------------------------------------------------
        // خروجی: شیء TaskEntity در صورت وجود، یا null اگر تسک حذف شده/یافت نشود.
        // در EF، FirstOrDefaultAsync بهترین روش برای کنترل وجود داده است.
        // -----------------------------------------------------------------------
        public async Task<TaskEntity?> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        // -----------------------------------------------------------------------
        // 🟩 متد ایجاد تسک جدید
        // -----------------------------------------------------------------------
        // ورودی: شیء TaskEntity آماده برای درج.
        // خروجی: شناسه رکورد اضافه‌شده پس از SaveChanges.
        // 💡 EF Core پس از SaveChanges مقدار Id را از دیتابیس برمی‌گرداند.
        // -----------------------------------------------------------------------
        public async Task<int> CreateAsync(TaskEntity entity)
        {
            _context.Tasks.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        // -----------------------------------------------------------------------
        // ✳️ متد بروزرسانی تسک موجود
        // -----------------------------------------------------------------------
        // منطق:
        // 1️⃣ جستجو تسک فعلی بر اساس Id.
        // 2️⃣ اگر حذف‌شده بود یا وجود نداشت → خروج با مقدار 0.
        // 3️⃣ در غیر این صورت، بروزرسانی مقدار‌ها و ذخیره تغییرات.
        //
        // 💬 خروجی: عدد 1 برای موفق، 0 برای ناموفق.
        // -----------------------------------------------------------------------
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

        // -----------------------------------------------------------------------
        // 🗑 متد حذف نرم (Soft Delete)
        // -----------------------------------------------------------------------
        // توضیح آموزشی RiRaDocs:
        //     ▫ Soft Delete به معنی عدم حذف رکورد از جدول است.
        //     ▫ فقط فیلد IsDeleted=true تنظیم می‌شود تا در کوئری‌های بعدی نمایش داده نشود.
        //     ▫ این روش برای حفظ تاریخچه در گزارش‌های مدیریتی ضروری است.
        //
        // خروجی: true اگر رکورد یافت و به‌درستی حذف نرم شد.
        // -----------------------------------------------------------------------
        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null) return false;

            task.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }

    // ===============================================================================
    // 📘 خلاصه آموزشی RiRaDocs Teaching Edition
    // ------------------------------------------------------------------------------
    // 🔹 جایگاه TaskRepository در معماری تمیز:
    //     ▫ قرارگیری در لایه Persistence برای ارتباط مستقیم با دیتابیس.
    //     ▫ اجرای قرارداد ITaskRepository از لایه Application.
    //
    // 🔹 وابستگی‌ها:
    //     ▫ AppDbContext → مدیریت مجموعه داده‌ها و ارتباط با EF Core.
    //     ▫ TaskEntity → تعریف دامنه تسک‌ها.
    //
    // 🔹 اصول اجرایی رعایت‌شده:
    //     ▫ Repository Pattern برای جداسازی لایه داده از منطق برنامه.
    //     ▫ Asynchronous Operations برای بهینه‌سازی عملکرد I/O.
    //     ▫ Soft Delete برای حفظ تمامیت داده در گزارشات تحلیلی.
    //
    // 🔹 نکته کاربردی:
    //     ▫ در صورت نیاز به فیلترهای خاص، از Specification Pattern می‌توان بهره برد.
    //
    // 🔹 تگ انتشار RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===============================================================================
}
