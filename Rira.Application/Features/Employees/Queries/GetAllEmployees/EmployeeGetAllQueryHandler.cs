using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.DTOs;
using Rira.Application.Base.Handler;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rira.Application.Features.Employees.Queries.GetAllEmployees
{
    // 🎯 هندلر اصلی برای Query: EmployeeGetAllQuery (CQRS + MediatR)
    // ---------------------------------------------------------------------
    // این کلاس در لایه‌ی Application تعریف شده و منطق واکشی داده‌ها از دیتابیس را اجرا می‌کند.
    // این Handler مسئول بخش "خواندن داده‌ها" در معماری CQRS است (یعنی Query).
    //
    // 📦 وظیفه:
    //   ▫ دریافت دستور Query از MediatR Pipeline.
    //   ▫ واکشی همه‌ی رکوردهای کارمند از DbContext پروژه.
    //   ▫ تبدیل داده‌های Entity به DTO با استفاده از AutoMapper.
    //   ▫ بازگرداندن نتیجه در قالب ResponseModel استاندارد (Success یا NotFound یا Fail).
    //
    // 🔹 ارث‌بری از BaseHandler:
    //   این کلاس متدهای کمکی مانند Success(), Fail(), NotFound() را ارائه می‌دهد
    //   تا پاسخ‌ها در قالب یکنواخت و استاندارد برگردانده شوند.
    //
    // 🔸 خروجی Handler: ResponseModel<List<EmployeeDTO>>
    public class EmployeeGetAllQueryHandler
        : BaseHandler, IRequestHandler<EmployeeGetAllQuery, ResponseModel<List<EmployeeDTO>>>
    {
        private readonly IMapper _mapper;

        // 💉 سازنده: دریافت وابستگی‌های موردنیاز از DI Container
        // ---------------------------------------------------------------------
        // IAppDbContext → دسترسی به لایه‌ی داده‌ها (EF Core)
        // IMapper → نگاشت Entity به DTO با کمترین کد دستی
        public EmployeeGetAllQueryHandler(IAppDbContext dbContext, IMapper mapper)
            : base(dbContext)
        {
            _mapper = mapper;
        }

        // ⚙️ متد اصلی Handle — اجرای Query از طریق MediatR
        // ---------------------------------------------------------------------
        // این متد به‌صورت Async پیاده‌سازی شده و تمام کارمندان را واکشی می‌کند.
        // جریان کلی اجرا:
        //   1️⃣ واکشی داده‌ها با EF Core (بدون Tracking، چون حالت خواندن است)
        //   2️⃣ بررسی تهی بودن لیست
        //   3️⃣ تبدیل به مدل DTO با AutoMapper
        //   4️⃣ برگرداندن نتیجه با وضعیت مناسب (موفق، خالی، یا خطا)
        public async Task<ResponseModel<List<EmployeeDTO>>> Handle(
            EmployeeGetAllQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // 🔍 مرحله ۱: واکشی تمام رکوردهای کارمند از DbSet
                // ------------------------------------------------------------
                // AsNoTracking باعث افزایش سرعت در عملیات خواندن می‌شود
                // چون تغییرات در این مرحله نیاز به Tracking ندارد.
                var employees = await _dbContext.Employees
                    .AsNoTracking()
                    .OrderByDescending(e => e.Id)
                    .ToListAsync(cancellationToken);

                // 🔍 مرحله ۲: بررسی عدم وجود نتیجه
                if (employees == null || employees.Count == 0)
                    return NotFound<List<EmployeeDTO>>("هیچ کارمندی در سیستم ثبت نشده است.");

                // 🧩 مرحله ۳: نگاشت داده‌ها به DTO
                var mappedDtos = _mapper.Map<List<EmployeeDTO>>(employees);

                // ✅ مرحله ۴: بازگرداندن پاسخ موفق
                return Success(mappedDtos, "لیست تمام کارمندان با موفقیت بازیابی شد.");
            }
            catch (Exception ex)
            {
                // ⚠️ مدیریت خطا
                // ------------------------------------------------------------
                // هر نوع استثناء (SQL، Mapper یا هر مورد دیگر) به پاسخ Fail استاندارد تبدیل می‌شود.
                return Fail<List<EmployeeDTO>>($"خطا در دریافت لیست کارمندان: {ex.Message}");
            }
        }

        // ===========================================================================================
        // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
        // -------------------------------------------------------------------------------------------
        // 🔹 ساختار کلی Handler در CQRS:
        //     ▫ QueryHandler فقط داده‌ها را "می‌خواند" (Read-only).
        //     ▫ CommandHandler داده‌ها را "تغییر" می‌دهد (Write).
        //
        // 🔹 نقش ابزارها در این کلاس:
        //     ▫ MediatR      → فراخوانی Handler بدون وابستگی مستقیم.
        //     ▫ EF Core      → دسترسی به داده‌ها و اجرای LINQ Queryها.
        //     ▫ AutoMapper   → نگاشت سریع Entity → DTO جهت ارسال به Presentation.
        //     ▫ ResponseModel → قالب‌بندی خروجی با وضعیت مجزا (Success, Fail, NotFound).
        //
        // 🔹 مزیت الگو:
        //     ▫ تفکیک کامل Concern خواندن داده از منطق تغییر آن.
        //     ▫ افزایش تست‌پذیری و قابلیت نگهداری سیستم.
        //     ▫ کدها تمیزتر، کوتاه‌تر، و قابل درک‌تر برای توسعه‌دهندگان جدید.
        //
        // 🔹 نکته‌ی معماری:
        //     در پروژه ریرا می‌توان مشابه این Handler برای فیلترهای خاص یا صفحه‌بندی توسعه داد،
        //     مثلاً: GetActiveEmployeesQueryHandler یا GetPagedEmployeesQueryHandler.
        //
        // 🔹 هیچ تغییر اجرایی در کد ایجاد نشده — فقط توضیحات آموزشی افزوده شدند.
        // ===========================================================================================
    }
}
