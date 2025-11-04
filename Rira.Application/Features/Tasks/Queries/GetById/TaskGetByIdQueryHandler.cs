using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.DTOs;
using Rira.Application.Interfaces;

namespace Rira.Application.Features.Tasks.Queries.GetById
{
    // 🧩 کلاس هندلر کوئری دریافت تسک بر اساس شناسه — TaskGetByIdQueryHandler
    // -------------------------------------------------------------------
    // این کلاس وظیفهٔ دریافت شناسه‌ی مورد نظر و واکشی رکورد متناظر از جدول Tasks را دارد.
    // معماری بر اساس CQRS طراحی شده و در این قسمت فقط عملیات "خواندن (Read)" انجام می‌شود.
    //
    // 📌 نقش در معماری RiRa:
    //   ▫ QueryHandler = منطق خواندن داده (Read Layer)
    //   ▫ MediatR به عنوان واسط پیام بین Controller و Handler عمل می‌کند.
    //   ▫ EF Core برای واکشی داده‌ها از دیتابیس استفاده می‌شود.
    //   ▫ AutoMapper برای تبدیل Entity به DTO جهت جلوگیری از افشای مدل داخلی به کلاینت‌ها.
    public class TaskGetByIdQueryHandler : IRequestHandler<TaskGetByIdQuery, ResponseModel<TaskDto>>
    {
        private readonly IAppDbContext _context;  // دسترسی به داده‌های برنامه (DbContext لایه Application)
        private readonly IMapper _mapper;          // Mapper برای تبدیل entity ↔ DTO

        // 💉 سازنده کلاس — تزریق وابستگی‌ها
        // -------------------------------------------------------------------
        // DbContext و AutoMapper از طریق Dependency Injection وارد می‌شوند.
        // این باعث افزایش تست‌پذیری و جداسازی وابستگی‌ها از منطق اصلی می‌شود.
        public TaskGetByIdQueryHandler(IAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ⚙️ متد Handle — منطق اصلی اجرای کوئری GetById
        // -------------------------------------------------------------------
        // این متد توسط MediatR هنگام ارسال Query از Controller صدا زده می‌شود.
        // ورودی: TaskGetByIdQuery (دارای شناسه Id)
        // خروجی: ResponseModel<TaskDto>
        //
        // گام‌ها:
        //   1️⃣ بررسی رکوردی با Id مشخص و حذف‌نشده در دیتابیس.
        //   2️⃣ اگر یافت نشد → بازگرداندن پاسخ با وضعیت 404.
        //   3️⃣ اگر یافت شد → تبدیل entity به DTO و بازگرداندن در پاسخ موفق.
        public async Task<ResponseModel<TaskDto>> Handle(TaskGetByIdQuery request, CancellationToken cancellationToken)
        {
            // 🧮 گام ۱: واکشی رکورد از دیتابیس
            // -------------------------------------------------------------------
            // AsNoTracking باعث بهبود کارایی می‌شود، چون در اینجا نیازی به ردیابی تغییرات نیست.
            var entity = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

            // ⚠️ گام ۲: بررسی عدم وجود رکورد
            // -------------------------------------------------------------------
            // اگر رکورد یافت نشود، ResponseModel با پیام خطای 404 برگردانده می‌شود.
            if (entity == null)
                return new ResponseModel<TaskDto>(false, "تسک مورد نظر یافت نشد.", 404, null);

            // 🔁 گام ۳: نگاشت Entity به DTO
            // -------------------------------------------------------------------
            // با AutoMapper داده‌ها فقط در حد لازم برای نمایش به کلاینت منتقل می‌شوند.
            var dto = _mapper.Map<TaskDto>(entity);

            // 🟢 گام ۴: آماده‌سازی پاسخ موفق
            // -------------------------------------------------------------------
            // خروجی در قالب ResponseModel<TaskDto> فرم‌دهی می‌شود تا ساختار پاسخ در کل پروژه یکسان باشد.
            return new ResponseModel<TaskDto>(true, "جزئیات تسک با موفقیت بازیابی شد.", 200, dto);
        }

        // ===========================================================================================
        // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
        // -------------------------------------------------------------------------------------------
        // 🔹 مسئولیت:
        //     ▫ واکشی رکورد تسک بر اساس شناسه (Id) از دیتابیس.
        //
        // 🔹 اصول معماری مرتبط:
        //     ▫ Clean Architecture: بدون وابستگی مستقیم به EF در سطح Controller.
        //     ▫ CQRS: جداسازی کامل عملیات Read از Write.
        //     ▫ MediatR: اجرای جریان درخواست از طریق Pattern پیام‌رسانی داخلی.
        //
        // 🔹 اجزای کلیدی:
        //     ▫ AsNoTracking برای افزایش سرعت خواندن.
        //     ▫ FirstOrDefaultAsync برای دریافت تنها یک رکورد مطایق شرط.
        //     ▫ AutoMapper برای تبدیل Domain Model به DTO.
        //
        // 🔹 وضعیت‌های خروجی:
        //     ▫ 200 → موفق: تسک یافت شد و بازگردانده شد.
        //     ▫ 404 → ناموفق: تسک با شناسه واردشده وجود ندارد یا حذف شده است.
        //
        // 🔹 تست‌پذیری:
        //     ▫ امکان Mock کردن IAppDbContext و تزریق آن برای Unit Test.
        //     ▫ بررسی صحت داده بازگشتی و پیام خروجی بدون نیاز به دیتابیس واقعی.
        //
        // 🔹 نکته آموزشی:
        //     ▫ QueryHandler فقط خواندن انجام می‌دهد، هیچ تغییر در داده (SaveChanges) نباید داشته باشد.
        //
        // 🔹 تگ انتشار RiRaDocs:
        //     RiraDocs-v2025.11.4-Stable-Final-Fixed
        // ===========================================================================================
    }
}
