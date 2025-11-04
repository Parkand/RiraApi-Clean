using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;

namespace Rira.Application.Features.Employees.Commands.DeleteEmployee
{
    // ⚙️ هندلر فرمان حذف کارمند از سیستم (CQRS + MediatR)
    // ------------------------------------------------------------
    // این کلاس وظیفه‌ی اجرای عمل حذف (Delete) روی موجودیت Employee را دارد.
    // MediatR پس از ارسال EmployeeDeleteCommand از لایه‌ی Application به این Handler،
    // آن را اجرا کرده و نتیجه را در قالب ResponseModel<int> برمی‌گرداند.
    public class EmployeeDeleteCommandHandler : IRequestHandler<EmployeeDeleteCommand, ResponseModel<int>>
    {
        // 💉 تزریق وابستگی به DbContext از طریق اینترفیس IAppDbContext
        // ------------------------------------------------------------
        // این روش وابستگی مستقیم Handler به لایه‌ی Infrastructure را از بین برده
        // و تست‌پذیری سیستم را بسیار افزایش می‌دهد.
        private readonly IAppDbContext _context;

        public EmployeeDeleteCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        // 🎯 متد Handle — نقطه‌ی اجرای عملیات حذف
        // ------------------------------------------------------------
        // این متد ابتدا بررسی می‌کند که آیا کارمند موردنظر در بانک اطلاعاتی وجود دارد یا خیر.
        // سپس در صورت وجود، عملیات حذف انجام و نتیجه بازگردانده می‌شود.
        public async Task<ResponseModel<int>> Handle(EmployeeDeleteCommand request, CancellationToken cancellationToken)
        {
            // 🔍 جستجوی کارمند بر اساس شناسه
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            // اگر کارمند با شناسه داده‌شده پیدا نشود:
            if (employee == null)
                return ResponseModel<int>.NotFound($"❌ کارمند با شناسه {request.Id} یافت نشد.");

            try
            {
                // 🗑 حذف رکورد و ذخیره تغییرات
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync(cancellationToken);

                // ✅ نتیجه موفقیت‌آمیز
                return ResponseModel<int>.Ok(employee.Id, "✅ کارمند با موفقیت از سیستم حذف شد.");
            }
            catch (Exception ex)
            {
                // ⚠️ مدیریت خطاها (مثل خطای اتصال یا محدودیت کلید خارجی)
                // ------------------------------------------------------------
                // پیام خطا در پاسخ Fail نگهداری می‌شود تا در لایه‌ی بالاتر لاگ گردد.
                return ResponseModel<int>.Fail($"خطا در حذف کارمند: {ex.Message}");
            }
        }

        // ===========================================================================================
        // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
        // -------------------------------------------------------------------------------------------
        // 🔹 این کلاس بخشی از الگوی CQRS است که فرمان "Delete" را از طریق MediatR اجرا می‌کند.
        // 🔹 جریان کاری:
        //     (1) Controller → EmployeeDeleteCommand → MediatR → Handler  
        //     (2) Handler → بررسی وجود کارمند → حذف → ذخیره تغییرات در DbContext  
        //     (3) ResponseModel<int> → انتقال نتیجه به Controller
        // 🔹 از متد FirstOrDefaultAsync برای یافتن دقیق رکورد استفاده شده است.
        // 🔹 استفاده از ResponseModel باعث استانداردسازی پاسخ‌ها در سطح کل پروژه می‌شود.
        // 🔹 الگوی try/catch باعث حفظ پایداری سیستم هنگام بروز خطای دیتابیس می‌گردد.
        // 🔹 تزریق وابستگی (IAppDbContext) باعث امکان تست واحد (Unit Testing) می‌شود.
        // 🔹 هیچ تغییری در منطق کد ایجاد نشده — صرفاً مستندسازی آموزشی فارسی اضافه شده است.
        // ===========================================================================================
    }
}
