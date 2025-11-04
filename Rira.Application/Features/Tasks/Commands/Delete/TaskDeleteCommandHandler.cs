using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.Interfaces;

namespace Rira.Application.Features.Tasks.Commands.Delete
{
    // 🧩 هندلر حذف تسک (Task Delete Command Handler)
    // -------------------------------------------------------------------
    // این کلاس مسئول اجرای فرآیند حذف یک تسک از سیستم است.
    // در معماری CQRS، CommandHandler بخش "نوشتنی (Write)" است که تغییر در داده‌ها را اعمال می‌کند.
    // در اینجا از "Soft Delete" استفاده شده، یعنی رکورد فیزیکی حذف نمی‌شود
    // بلکه فیلد IsDeleted مقدار true می‌گیرد تا در واکشی‌ها مخفی شود.
    //
    // ⚙️ فرآیند اجرا به ترتیب:
    //   1️⃣ کنترلر یا API → ارسال Command با شناسه (TaskDeleteCommand)
    //   2️⃣ MediatR → فراخوانی TaskDeleteCommandHandler.Handle()
    //   3️⃣ Handler → جست‌وجو در DbContext، بررسی وجود تسک، و اعمال حذف نرم
    //
    // 📬 خروجی:
    //   ResponseModel<int> → وضعیت، پیام، کد پاسخ (HTTP)، و شناسهٔ تسک حذف‌شده
    //
    // 🧠 وابستگی:
    //   IAppDbContext از طریق DI تزریق شده تا Handler مستقیماً وابسته به EF Core نباشد
    public class TaskDeleteCommandHandler
        : IRequestHandler<TaskDeleteCommand, ResponseModel<int>>
    {
        private readonly IAppDbContext _context;

        // 💉 سازنده کلاس — تزریق DbContext از بیرون
        // -------------------------------------------------------------------
        // به‌جای استفاده مستقیم از DbContext اصلی پروژه‌ی EF Core،
        // این پروژه از واسط IAppDbContext برای افزایش تست‌پذیری استفاده می‌کند.
        public TaskDeleteCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        // ⚙️ متد Handle — منطق اصلی حذف تسک (Soft Delete)
        // -------------------------------------------------------------------
        // ورودی: TaskDeleteCommand شامل شناسه تسک
        // گام‌ها:
        //   1️⃣ یافتن تسک فعال (که حذف نشده باشد)
        //   2️⃣ در صورت پیدا نکردن → بازگشت پیام خطای 404
        //   3️⃣ در غیر این صورت → تنظیم IsDeleted = true و ذخیره تغییرات
        //   4️⃣ بازگرداندن پاسخ استاندارد با شناسه تسک حذف‌شده
        public async Task<ResponseModel<int>> Handle(TaskDeleteCommand request, CancellationToken cancellationToken)
        {
            // 🧩 مرحله ۱: جست‌وجوی تسک در پایگاه داده، فقط میان تسک‌هایی که هنوز حذف نشده‌اند
            var entity = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

            // 🚫 در صورت عدم یافتن رکورد فعال مورد نظر
            // -------------------------------------------------------------------
            // ممکن است تسک از قبل حذف شده یا اصلاً وجود نداشته باشد.
            // در این صورت پیام خطای 404 و داده خالی (Id=0) بازگردانده می‌شود.
            if (entity == null)
                return new ResponseModel<int>(false, "تسک یافت نشد یا قبلاً حذف شده.", 404, 0);

            // 🗑️ مرحله ۲: انجام "Soft Delete"
            // -------------------------------------------------------------------
            // حذف نرم یعنی فقط تغییر وضعیت منطقی بدون پاک‌کردن داده از پایگاه داده.
            // این روش باعث حفظ تاریخچه و امکان بازگردانی داده در آینده می‌شود.
            entity.IsDeleted = true;

            // 🧾 ذخیره تغییرات در پایگاه داده
            await _context.SaveChangesAsync(cancellationToken);

            // ✅ مرحله ۳: بازگرداندن پاسخ موفقیت‌آمیز با شناسه تسک حذف‌شده
            return new ResponseModel<int>(true, "تسک با موفقیت حذف شد (Soft Delete).", 200, entity.Id);
        }

        // ===========================================================================================
        // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
        // -------------------------------------------------------------------------------------------
        // 🔹 جایگاه کلاس:
        //     Application → Features → Tasks → Commands → Delete → TaskDeleteCommandHandler.cs
        //
        // 🔹 نقش در CQRS:
        //     ▫ یک CommandHandler است (در بخش Write).
        //     ▫ اتفاق حذف را در DbContext اعمال می‌کند.
        //
        // 🔹 نکته معماری:
        //     ▫ به‌جای حذف فیزیکی رکورد، ویژگی IsDeleted را تغییر می‌دهیم.
        //       این کار هم از نظر ایمنی داده بهتر است و هم گزارشات تاریخی را حفظ می‌کند.
        //
        // 🔹 نکات فنی:
        //     ▫ استفاده از async/await برای I/O غیربلاک‌کننده.
        //     ▫ FirstOrDefaultAsync برای یافتن رکورد خاص با توجه به CancellationToken.
        //
        // 🔹 نکته آزمایش (Testability):
        //     ▫ می‌توان با "Mock کردن" IAppDbContext تست واحد (Unit Test) انجام داد.
        //     ▫ سناریوها:
        //         - حذف موفق (با یافتن تسک)
        //         - تلاش برای حذف تسک حذف‌شده
        //         - تلاش برای حذف Id اشتباه (NotFound)
        //
        // 🔹 برگرداندن ResponseModel<int>:
        //     ▫ نتیجهٔ عملیات به فرم استاندارد و قابل تفسیر برای Controller.
        //     ▫ Success → true یا false
        //     ▫ Message → توصیفِ نتیجه (موفق یا خطا)
        //     ▫ StatusCode → 200 / 404
        //     ▫ Data → شناسهٔ تسک هدف
        //
        // 🔹 هیچ تغییری در منطق کد اجرایی داده نشده — فقط توضیحات فارسی آموزشی افزوده شدند.
        // ===========================================================================================
    }
}
