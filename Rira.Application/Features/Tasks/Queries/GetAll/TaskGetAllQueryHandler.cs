using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.DTOs;
using Rira.Application.Interfaces;

namespace Rira.Application.Features.Tasks.Queries.GetAll
{
    // 🧩 هندلر کوئری دریافت همه‌ی تسک‌ها — TaskGetAllQueryHandler
    // -------------------------------------------------------------------
    // این کلاس مسئول اجرای فرآیند "خواندن" داده‌های تسک‌ها از پایگاه داده است.
    //
    // 📌 جایگاه در معماری:
    //     در الگوی CQRS، بخش QueryHandlerها مخصوص عملیات Read هستند و هیچ تغییر یا ذخیره‌ای در داده‌ها انجام نمی‌دهند.
    //
    // ⚙️ جریان کلی عملیات:
    //     1️⃣ Controller متد GET را صدا می‌زند.
    //     2️⃣ MediatR یک نمونه TaskGetAllQuery ارسال می‌کند.
    //     3️⃣ این هندلر متد Handle را اجرا می‌کند تا داده‌ها از DbContext واکشی شوند.
    //     4️⃣ AutoMapper داده‌های مدل را به DTOها تبدیل می‌کند.
    //     5️⃣ نتیجه در قالب ResponseModel<List<TaskDto>> به Controller بازگردانده می‌شود.
    //
    // 🎯 هدف آموزشی:
    //     نمایش نحوه‌ی جداسازی خواندن داده‌ها از منطق دیگر در معماری تمیز، 
    //     همراه با آموزش کاربرد AsNoTracking و AutoMapper.
    public class TaskGetAllQueryHandler
        : IRequestHandler<TaskGetAllQuery, ResponseModel<List<TaskDto>>>
    {
        private readonly IAppDbContext _context;
        private readonly IMapper _mapper;

        // 💉 سازنده کلاس — تزریق وابستگی‌ها
        // -------------------------------------------------------------------
        // این هندلر برای دسترسی به داده‌ها از طریق IAppDbContext و تبدیل آن‌ها از طریق AutoMapper تعریف شده است.
        public TaskGetAllQueryHandler(IAppDbContext context, IMapper mapper)
        {
            _context = context;   // دسترسی به دیتابیس از طریق لایهٔ انتزاعی Application
            _mapper = mapper;     // برای تبدیل Entityها به Data Transfer Object (DTO)
        }

        // ⚙️ متد اصلی Handle — اجرای کوئری دریافت همه‌ی تسک‌ها
        // -------------------------------------------------------------------
        // این متد هنگام دریافت Query از MediatR فراخوانی می‌شود و داده‌ها را برمی‌گرداند.
        //
        // ورودی: TaskGetAllQuery
        // خروجی: ResponseModel<List<TaskDto>>
        //
        // مراحل:
        //   1️⃣ واکشی لیست تسک‌ها از DbContext با AsNoTracking و حذف‌شده‌ها را فیلتر می‌کند.
        //   2️⃣ مرتب‌سازی نزولی بر اساس شناسه (Id).
        //   3️⃣ تبدیل لیست Entityها به لیست DTOها با AutoMapper.
        //   4️⃣ بازگرداندن پاسخ استاندارد با پیام و کد وضعیت.
        public async Task<ResponseModel<List<TaskDto>>> Handle(TaskGetAllQuery request, CancellationToken cancellationToken)
        {
            // 🧮 مرحله ۱: واکشی داده‌ها
            // -------------------------------------------------------------------
            // AsNoTracking برای افزایش سرعت خواندن داده‌ها استفاده می‌شود زیرا نیازی به ردیابی تغییرات نیست.
            var tasks = await _context.Tasks
                .AsNoTracking()
                .Where(t => !t.IsDeleted)             // حذف منطقی (Soft Delete)
                .OrderByDescending(t => t.Id)         // مرتب‌سازی از جدیدترین به قدیمی‌ترین
                .ToListAsync(cancellationToken);

            // ✍️ مرحله ۲: تبدیل داده‌ها به DTO
            // -------------------------------------------------------------------
            // AutoMapper کار تبدیل Entity (مدل پایگاه داده) به DTO نمایش را انجام می‌دهد.
            var dtoList = _mapper.Map<List<TaskDto>>(tasks);

            // 🟢 مرحله ۳: بازگرداندن پاسخ نهایی
            // -------------------------------------------------------------------
            // ResponseModel یک ساختار پاسخ استاندارد در RiRa است با وضعیت، پیام و داده‌ها.
            return new ResponseModel<List<TaskDto>>(
                true,
                "لیست تسک‌ها با موفقیت بازیابی شد.",
                200,
                dtoList
            );
        }

        // ===========================================================================================
        // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
        // -------------------------------------------------------------------------------------------
        // 🔹 مسئولیت کلاس:
        //     ▫ اجرای منطق خواندن (Read Operation) برای موجودیت Task.
        //     ▫ بازگرداندن داده‌ها بدون تغییر در حالت برنامه.
        //
        // 🔹 ویژگی‌های کلیدی:
        //     ▫ AsNoTracking → بهینه‌سازی سرعت خواندن و مصرف حافظه.
        //     ▫ AutoMapper → تبدیل سریع و تمیز بین Entity و DTO.
        //     ▫ Soft Delete → فیلتر رکوردهای حذف‌شده (IsDeleted == true).
        //
        // 🔹 پاسخ خروجی:
        //     ResponseModel<List<TaskDto>>
        //         success = true
        //         message = "لیست تسک‌ها با موفقیت بازیابی شد."
        //         statusCode = 200
        //         data = فهرست تسک‌ها (DTO)
        //
        // 🔹 نکات معماری:
        //     ▫ اصل جداسازی مسئولیت‌ها: Queryها فقط خواندن انجام می‌دهند.
        //     ▫ وابستگی از طریق IAppDbContext باعث تست‌پذیری هندلر می‌شود.
        //
        // 🔹 آزمودنی بودن (Testability):
        //     ▫ می‌توان با Mock کردن DbContext و Mapper خروجی را بررسی کرد.
        //     ▫ بررسی تعداد تسک‌های بازگردانده‌شده یا صحت نگاشت Mapper ممکن است.
        //
        // 🔹 بدون تغییر اجرایی در منطق برنامه — فقط توضیحات آموزشی فارسی اضافه شده‌اند.
        // ===========================================================================================
    }
}
