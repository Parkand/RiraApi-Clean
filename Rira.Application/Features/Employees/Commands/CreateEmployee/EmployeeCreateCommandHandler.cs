using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;

namespace Rira.Application.Features.Employees.Commands.CreateEmployee
{
    // ⚙️ هندلر فرمان ایجاد کارمند جدید (CQRS + MediatR)
    // ------------------------------------------------------------
    // این کلاس منطق اجرایی فرمان EmployeeCreateCommand را پیاده‌سازی می‌کند.
    // نقش Handler در معماری CQRS، اجرای دقیق درخواست (Command) ارسال‌شده از Application Layer است.
    // این Handler پس از ولیدیشن یکتایی داده، Command را به موجودیت (Entity) نگاشت می‌کند
    // و نتیجه‌ی عملیات دیتابیس را در قالب ResponseModel بازمی‌گرداند.
    public class EmployeeCreateCommandHandler : IRequestHandler<EmployeeCreateCommand, ResponseModel<int>>
    {
        // تزریق وابستگی‌ها (Dependency Injection)
        // ------------------------------------------------------------
        // IAppDbContext → برای ارتباط با دیتابیس (DbSet<EmployeeEntity>)
        // IMapper → برای نگاشت خودکار Command به Entity
        private readonly IAppDbContext _context;
        private readonly IMapper _mapper;

        // سازنده DI
        public EmployeeCreateCommandHandler(IAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // 💡 متد Handle در الگوی MediatR به عنوان نقطه‌ی ورود Command عمل می‌کند.
        // هر Command دقیقاً یک Handler دارد و Handler خروجی را با نوع پیش‌بینی‌شده بازمی‌گرداند.
        public async Task<ResponseModel<int>> Handle(EmployeeCreateCommand request, CancellationToken cancellationToken)
        {
            // 🕵 بررسی تکراری بودن ایمیل یا شماره تماس
            // ------------------------------------------------------------
            // قبل از ایجاد کارمند جدید، اطمینان حاصل می‌شود که ایمیل یا شماره موبایل قبلاً ثبت نشده باشند.
            bool duplicateExists = await _context.Employees
                .AnyAsync(emp =>
                    emp.Email == request.Email ||
                    emp.MobileNumber == request.MobileNumber, cancellationToken);

            // در صورت وجود داده تکراری، عملیات متوقف و پیام خطا بازگردانده می‌شود.
            if (duplicateExists)
                return ResponseModel<int>.Fail("❌ ایمیل یا شماره موبایل تکراری است.");

            try
            {
                // 🧩 نگاشت Command به Entity
                // ------------------------------------------------------------
                // AutoMapper مقادیر فیلدهای کلاس Command را به EmployeeEntity تبدیل می‌کند.
                // این رویکرد خوانایی و سرعت توسعه را افزایش می‌دهد و از وابستگی مستقیم جلوگیری می‌کند.
                var employee = _mapper.Map<EmployeeEntity>(request);

                // 🗄 افزودن کارمند جدید به دیتابیس
                await _context.Employees.AddAsync(employee, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // ✅ بازگرداندن نتیجه موفق به همراه شناسه ثبت‌شده
                return ResponseModel<int>.Ok(employee.Id, "✅ کارمند جدید با موفقیت ایجاد شد.");
            }
            catch (Exception ex)
            {
                // ⚠️ مدیریت خطاهای احتمالی
                // ------------------------------------------------------------
                // در صورت بروز خطا هنگام درج داده در دیتابیس، پیام Fail برمی‌گردانده می‌شود.
                // جزئیات خطا در متن پیام ثبت می‌گردد تا در لایه بالاتر لاگ شود.
                return ResponseModel<int>.Fail($"خطا در ایجاد کارمند: {ex.Message}");
            }
        }

        // ===========================================================================================
        // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
        // -------------------------------------------------------------------------------------------
        // 🔹 این کلاس بخش مهمی از الگوی CQRS است و با MediatR به Command مرتبط می‌شود.
        // 🔹 وظیفه‌اش ایجاد کارمند جدید با استفاده از EF Core و AutoMapper است.
        // 🔹 تزریق IAppDbContext باعث جداسازی منطقPersistence از Handler می‌شود.
        // 🔹 در این Handler از AnyAsync برای بررسی یکتایی داده‌ها و از SaveChangesAsync برای ثبت نهایی استفاده شده است.
        // 🔹 ResponseModel نوع پاسخ استاندارد در کل پروژه ریرا است (شامل وضعیت، پیام و داده خروجی).
        // 🔹 اجرای Command‌ها از طریق MediatR باعث افزایش تست‌پذیری و قابلیت نگهداری پروژه می‌شود.
        // 🔹 هیچ تغییر اجرایی در منطق این کلاس صورت نگرفته — فقط توضیحات آموزشی فارسی اضافه شده‌اند.
        // ===========================================================================================
    }
}
