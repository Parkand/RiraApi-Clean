using MediatR;
using Rira.Application.Common;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;

namespace Rira.Application.Features.Tasks.Commands.Create
{
    // 🎯 هندلر اصلی برای فرمان ایجاد تسک جدید (CQRS + MediatR)
    // -------------------------------------------------------------------
    // این کلاس مسئول اجرای منطق ثبت رکورد جدید از نوع TaskEntity در پایگاه داده است.
    // بخش «CommandHandler» در معماری CQRS وظیفه‌ی اعمال تغییرات (write operations)
    // را دارد و برخلاف QueryHandler هیچ داده‌ای واکشی نمی‌کند.
    //
    // 📦 این کلاس مستقیماً توسط MediatR Pipeline فراخوانی می‌شود،
    // زمانیکه کنترلر مربوطه Command را ارسال کند:
    //      ➜ TaskCreateCommand → MediatR → TaskCreateCommandHandler
    //
    // ✳️ خروجی:
    //      ResponseModel<int>
    //      شامل شناسه تسک ایجاد شده و پیام موفقیت یا خطا می‌باشد.
    //
    // 🧩 وابستگی‌ها:
    //      ▫ IAppDbContext → لایه‌ی ارتباط با EF Core برای افزودن رکورد.
    //      ▫ CancellationToken → کنترل فرایند async برای درخواست‌های طولانی.
    public class TaskCreateCommandHandler
        : IRequestHandler<TaskCreateCommand, ResponseModel<int>>
    {
        private readonly IAppDbContext _context;

        // 💉 سازنده‌ی کلاس — تزریق وابستگی‌ها از طریق DI Container
        // -------------------------------------------------------------------
        // DbContext پیاده‌سازی‌شده در پروژه Rira.Application به هندلر تزریق می‌شود.
        // این وابستگی اجازه می‌دهد رکورد جدید در جدول Tasks نوشته شود.
        public TaskCreateCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        // ⚙️ متد اصلی Handle — اجرای منطق ایجاد تسک جدید
        // -------------------------------------------------------------------
        // گام‌های اجرایی:
        //   1️⃣ ساخت نمونهٔ جدید از TaskEntity بر اساس داده‌های ورودی در Command.
        //   2️⃣ افزودن این نمونه به DbSet مربوط به Tasks.
        //   3️⃣ ذخیرهٔ تغییرات (SaveChangesAsync).
        //   4️⃣ بازگرداندن پاسخ ResponseModel با شناسه‌ی تسک ایجادشده.
        public async Task<ResponseModel<int>> Handle(TaskCreateCommand request, CancellationToken cancellationToken)
        {
            // 🧩 ایجاد آبجکت جدید از نوع TaskEntity بر اساس داده‌های دریافتی از Command
            // -------------------------------------------------------------------
            // در این مرحله، داده‌های خام (Title، Description، Priority، Status، DueDate)
            // مستقیماً از Command گرفته می‌شوند و در ساختار Entity قرار می‌گیرند.
            // مقادیر CreatedAt و IsDeleted نیز به صورت پیش‌فرض تعیین می‌شوند.
            var entity = new TaskEntity
            {
                Title = request.Title,                       // عنوان تسک
                Description = request.Description,           // توضیحات (اختیاری)
                Status = request.Status,                     // وضعیت جاری
                Priority = request.Priority,                 // اولویت کاری
                DueDate = request.DueDate,                   // تاریخ سررسید
                CreatedAt = DateTime.Now.ToString("yyyy/MM/dd"), // تاریخ ایجاد به فرمت استاندارد پروژه
                IsDeleted = false                            // وضعیت منطقی حذف (پیش‌فرض: فعال)
            };

            // 🗃️ ثبت تسک جدید در DbContext
            _context.Tasks.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            // ✅ بازگرداندن پاسخ موفقیت همراه با شناسهٔ تسک جدید
            return new ResponseModel<int>(true, "تسک با موفقیت ایجاد شد.", 201, entity.Id);
        }

        // ===========================================================================================
        // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
        // -------------------------------------------------------------------------------------------
        // 🔹 CommandHandler در CQRS:
        //     ▫ فقط برای "تغییر وضعیت" یا "ثبت داده جدید" در سیستم استفاده می‌شود.
        //     ▫ هیچ داده‌ای از دیتابیس نمی‌خواند (برخلاف QueryHandler).
        //
        // 🔹 جریان داده در این کلاس:
        //     [Controller] → Command(TaskCreate) → [MediatR] → Handler → [DbContext] → پاسخ استاندارد.
        //
        // 🔹 ResponseModel:
        //     کلاس عمومی برای قالب‌بندی پاسخ‌ها در RiRa.Application.
        //     شامل سه بخش است: وضعیت موفقیت، پیام، و داده/شناسه خروجی.
        //
        // 🔹 نکات کلیدی:
        //     ▫ استفاده از IAppDbContext باعث عدم وابستگی مستقیم به EF Core می‌شود.
        //       بنابراین تست‌پذیری کد بالا می‌رود.
        //     ▫ تاریخ ایجاد و فیلد IsDeleted کنترل‌های پیش‌فرض برای مدیریت چرخه‌ی تسک‌اند.
        //
        // 🔹 نکته آینده‌نگر:
        //     می‌توان Ruleهای اعتبارسنجی را در Validator مستقل (TaskCreateValidator.cs)
        //     اعمال کرد تا از ورود دادهٔ نامعتبر جلوگیری شود.
        //
        // 🔹 هیچ تغییر اجرایی در کد داده نشده — فقط توضیحات آموزشی فارسی افزوده شدند.
        // ===========================================================================================
    }
}
