using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;

namespace Rira.Application.Features.Employees.Commands.UpdateEmployee
{
    // ⚙️ هندلر فرمان ویرایش کارمند (CQRS + MediatR)
    // ------------------------------------------------------------
    // این کلاس مسئول اجرای عملیات "ویرایش اطلاعات کارمند" در لایه Application است.
    // پس از ارسال فرمان EmployeeUpdateCommand از Controller، MediatR آن را به این Handler هدایت می‌کند.
    // Handler با استفاده از EF Core رکورد هدف را از دیتابیس واکشی کرده، داده‌های جدید را توسط AutoMapper
    // روی مدل موجود (Entity) نگاشت می‌کند و در انتها تغییرات را ذخیره می‌نماید.
    //
    // این Pattern باعث جداسازی کامل منطق بروزرسانی از کنترلر و ایجاد ساختار تست‌پذیر در پروژه می‌شود.
    public class EmployeeUpdateCommandHandler : IRequestHandler<EmployeeUpdateCommand, ResponseModel<int>>
    {
        // 💉 تزریق وابستگی‌ها (Dependency Injection)
        // ------------------------------------------------------------
        // IAppDbContext: برای اتصال به DbContext اختصاصی پروژه
        // IMapper: برای نگاشت خودکار داده‌ها بین Command و Entity
        private readonly IAppDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeUpdateCommandHandler(IAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // 🎯 متد اصلی Handle — اجرای فرمان MediatR
        // ------------------------------------------------------------
        // این متد غیرهمزمان (Asynchronous) است تا کار با پایگاه داده به‌صورت ایمن و کارآمد انجام شود.
        // انجام مراحل:
        //   1️⃣ بررسی وجود رکورد
        //   2️⃣ نگاشت داده‌های جدید روی رکورد موجود
        //   3️⃣ ذخیره تغییرات و بازگرداندن نتیجه استاندارد به صورت ResponseModel
        public async Task<ResponseModel<int>> Handle(EmployeeUpdateCommand request, CancellationToken cancellationToken)
        {
            // 🔍 مرحله اول: بررسی وجود کارمند
            // ------------------------------------------------------------
            // رکورد هدف با استفاده از شناسه (Id) جستجو می‌شود.
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (employee == null)
            {
                // اگر رکورد یافت نشود، پاسخ NotFound بازگردانده می‌شود.
                return ResponseModel<int>.NotFound($"❌ کارمند با شناسه {request.Id} یافت نشد.");
            }

            try
            {
                // 🧩 مرحله دوم: نگاشت داده‌ها با AutoMapper
                // ------------------------------------------------------------
                // داده‌های موجود در فرمان (request) روی نمونه موجود از Entity اعمال می‌شوند.
                // AutoMapper فقط فیلدهای مقداردهی‌شده را تغییر می‌دهد و سایر مقادیر را حفظ می‌کند.
                _mapper.Map(request, employee);

                // 💾 مرحله سوم: ذخیره تغییرات در دیتابیس
                await _context.SaveChangesAsync(cancellationToken);

                // ✅ پاسخ موفق شامل شناسه‌ی کارمند و پیام موفقیت
                return ResponseModel<int>.Ok(employee.Id, $"✅ کارمند با شناسه {employee.Id} با موفقیت ویرایش شد.");
            }
            catch (Exception ex)
            {
                // ⚠️ مدیریت خطاهای احتمالی
                // ------------------------------------------------------------
                // هر نوع خطا (مثل خطای Mapper، خطای ارتباط با پایگاه داده یا تغییرات غیرمجاز)
                // منجر به برگرداندن پاسخ Fail می‌شود.
                return ResponseModel<int>.Fail($"خطا در بروزرسانی کارمند: {ex.Message}");
            }
        }

        // ===========================================================================================
        // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
        // -------------------------------------------------------------------------------------------
        // 🔹 این Handler نمونه‌ای از پیاده‌سازی CommandHandler در معماری CQRS است.
        // 🔹 مسئولیت آن فقط اجرای منطق بروزرسانی داده‌ها است — بدون هیچ Dependency به UI.
        // 🔹 ترکیب چهار ابزار کلیدی در این کلاس:
        //     ▫ MediatR → مدیریت جریان فرمان و پاسخ.
        //     ▫ EF Core → بررسی وجود و ذخیره داده‌ها.
        //     ▫ AutoMapper → نگاشت Command → Entity.
        //     ▫ ResponseModel → قالب استاندارد پاسخ‌دهی.
        // 🔹 این ساختار باعث کاهش وابستگی، افزایش خوانایی و تست‌پذیری کد می‌شود.
        // 🔹 هیچ تغییر اجرایی در منطق کد ایجاد نشده — فقط توضیحات آموزشی فارسی افزوده شده‌اند.
        // ===========================================================================================
    }
}
