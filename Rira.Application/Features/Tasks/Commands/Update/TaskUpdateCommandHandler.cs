using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.Interfaces;

namespace Rira.Application.Features.Tasks.Commands.Update
{
    // 🧩 هندلر به‌روزرسانی تسک (Task Update Command Handler)
    // -------------------------------------------------------------------
    // این کلاس مسئول اجرای فرآیند ویرایش یک تسک در سیستم است.
    //
    // 📌 جایگاه در معماری:
    //     در الگوی CQRS، CommandHandlerها منطق مربوط به عملیات "نوشتن"
    //     (یعنی تغییر داده‌ها در پایگاه داده) را اجرا می‌کنند.
    //
    // ⚙️ جریان کلی:
    //     1️⃣ کنترلر (TasksController) یک فرمان TaskUpdateCommand ارسال می‌کند.
    //     2️⃣ MediatR با پیدا کردن هندلر متناظر، متد Handle را فراخوانی می‌کند.
    //     3️⃣ این هندلر تسک را بر اساس Id جست‌وجو کرده و مقادیر جدید را اعمال می‌کند.
    //     4️⃣ پس از ذخیره، پاسخ استاندارد ResponseModel<int> بازگردانده می‌شود.
    //
    // 🧠 هدف آموزشی:
    //     نمایش نحوه‌ی به‌روزرسانی رکورد در معماری تمیز با حفظ جدایی مسئولیت‌ها.
    public class TaskUpdateCommandHandler
        : IRequestHandler<TaskUpdateCommand, ResponseModel<int>>
    {
        private readonly IAppDbContext _context;

        // 💉 سازنده کلاس — تزریق وابستگی‌ها
        // -------------------------------------------------------------------
        // واسط IAppDbContext از طریق DI تزریق می‌شود تا به‌صورت انتزاعی با دیتابیس کار کند.
        // این کار باعث افزایش تست‌پذیری و جدایی هندلر از لایه‌ی زیرساخت EF Core می‌شود.
        public TaskUpdateCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        // ⚙️ متد اصلی Handle — اجرای دستور به‌روزرسانی
        // -------------------------------------------------------------------
        // این متد توسط MediatR فراخوانی می‌شود و منطق اصلی به‌روزرسانی تسک را اجرا می‌نماید.
        //
        // ورودی: TaskUpdateCommand
        // خروجی: ResponseModel<int>
        //
        // مراحل:
        //   1️⃣ واکشی رکورد با Id و اعتبار حذف نشدن (Soft Delete)
        //   2️⃣ در صورت عدم وجود رکورد، بازگشت پیام خطا با StatusCode=404
        //   3️⃣ در صورت وجود، جایگزینی داده‌ها با مقادیر جدید Command
        //   4️⃣ ثبت زمان آخرین ویرایش (UpdatedAt)
        //   5️⃣ ذخیره‌ی تغییرات در دیتابیس
        //   6️⃣ بازگرداندن پاسخ موفقیت‌آمیز
        public async Task<ResponseModel<int>> Handle(TaskUpdateCommand request, CancellationToken cancellationToken)
        {
            // 🧮 مرحله ۱: واکشی تسک معتبر (غیر حذف‌شده)
            var entity = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

            // 🚫 اگر تسک یافت نشود → پاسخ خطا
            // -------------------------------------------------------------------
            // در صورت نبود رکورد با Id مشخص، عملیات متوقف و پیام خطا (404) برگردانده می‌شود.
            if (entity == null)
                return new ResponseModel<int>(false, "تسک مورد نظر یافت نشد.", 404, 0);

            // ✍️ مرحله ۲: جایگزینی داده‌های جدید
            // -------------------------------------------------------------------
            // مقادیر جدید از Command مستقیماً در رکورد موجود نوشته می‌شوند.
            entity.Title = request.Title;
            entity.Description = request.Description;
            entity.Status = request.Status;
            entity.Priority = request.Priority;
            entity.DueDate = request.DueDate;

            // 🕓 مرحله ۳: بروزرسانی تاریخ آخرین ویرایش
            // -------------------------------------------------------------------
            // در این پروژه تاریخ به‌صورت رشته‌ای (yyyy/MM/dd) ذخیره می‌شود تا با زبان فارسی سازگار باشد.
            entity.UpdatedAt = DateTime.Now.ToString("yyyy/MM/dd");

            // 💾 مرحله ۴: ذخیره‌ی تغییرات در پایگاه داده
            await _context.SaveChangesAsync(cancellationToken);

            // 🟢 مرحله ۵: بازگرداندن پاسخ موفقیت‌آمیز
            // -------------------------------------------------------------------
            // قالب پاسخ استاندارد با دادهٔ شناسهٔ تسک و پیام تأیید.
            return new ResponseModel<int>(true, "تسک با موفقیت ویرایش شد.", 200, entity.Id);
        }

        // ===========================================================================================
        // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
        // -------------------------------------------------------------------------------------------
        // 🔹 مسئولیت کلاس:
        //     ▫ اجرای منطق "Update" برای موجودیت Task.
        //     ▫ به‌روزرسانی داده‌های رکورد در DbContext.
        //
        // 🔹 بخش‌های کلیدی:
        //     ▫ FirstOrDefaultAsync: جست‌وجوی رکورد تسک با Id.
        //     ▫ Soft Delete: بررسی IsDeleted برای حذف منطقی.
        //     ▫ UpdatedAt: ذخیره آخرین تاریخ ویرایش با فرمت یکنواخت.
        //
        // 🔹 پاسخ (ResponseModel<int>):
        //     ▫ Success: نشان‌دهندهٔ موفق یا ناموفق بودن درخواست.
        //     ▫ Message: توضیح نتیجهٔ فرآیند برای کاربر.
        //     ▫ StatusCode: 200 (موفق) یا 404 (یافت نشد).
        //     ▫ Data: شناسه رکورد تغییر‌یافته.
        //
        // 🔹 نکته معماری:
        //     ▫ لایه Application فقط منطق بیزنس دارد، وابستگی مستقیم به Controller یا Repository ندارد.
        //     ▫ هندلرها قابل تست جداگانه هستند و از طریق DI تست می‌شوند.
        //
        // 🔹 تست‌پذیری:
        //     ▫ ورودی‌ها (Command) و خروجی‌ها (ResponseModel) ساده و مستقل‌اند.
        //     ▫ می‌توان با Mock کردن IAppDbContext صحت عملکرد هندلر را آزمود.
        //
        // 🔹 بدون هیچ تغییر اجرایی در منطق برنامه — تنها توضیحات آموزشی فارسی اضافه شده‌اند.
        // ===========================================================================================
    }
}
