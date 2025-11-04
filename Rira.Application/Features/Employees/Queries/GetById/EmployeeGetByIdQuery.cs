using MediatR;
using Rira.Application.Common;
using Rira.Application.DTOs;

namespace Rira.Application.Features.Employees.Queries.GetById
{
    // 🎯 Query اصلی برای دریافت یک کارمند با شناسه‌ی مشخص (CQRS + MediatR)
    // -----------------------------------------------------------------------------
    // این کلاس بخشی از معماری "Query Side" در الگوی CQRS است.
    // هدف آن درخواست واکشی یک رکورد از نوع Employee با استفاده از شناسه (Id) است.
    //
    // 📦 ساختار:
    //   ▫ از MediatR پیروی می‌کند تا Handler متناظر (EmployeeGetByIdQueryHandler)
    //     بتواند درخواست را به‌صورت Loose Coupled دریافت و پردازش کند.
    //
    // 🔹 خروجی این Query:
    //     ResponseModel<EmployeeDTO> 
    //   ⮕ شامل داده‌ی اصلی (EmployeeDTO) + وضعیت موفقیت/شکست + پیام توضیحی.
    //
    // 💡 نکته:
    //   این کلاس فقط “تعریف درخواست” است و "منطق اجرا" در Handler قرار دارد.
    public class EmployeeGetByIdQuery : IRequest<ResponseModel<EmployeeDTO>>
    {
        // 🆔 شناسه‌ی کارمند مورد نظر برای واکشی
        // ------------------------------------------------------------
        // مقدار این ویژگی معمولاً از مسیر (Route) در Controller دریافت می‌شود.
        // مثال: GET /api/employees/5 → Id = 5
        //
        // ⚠️ نکته معماری:
        // بهتر است قبل از اجرا در Handler یا Validator بررسی شود که Id > 0 باشد،
        // در غیر این صورت، پاسخ معتبر (BadRequest یا NotFound) بازگردانده خواهد شد.
        public int Id { get; set; }
    }

    // ===========================================================================================
    // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
    // -------------------------------------------------------------------------------------------
    // 🔹 کوئری‌ها در CQRS:
    //     ▫ مخصوص عملیات‌های "خواندن داده" از سیستم‌اند.
    //     ▫ هیچ تغییری در وضعیت دیتابیس ایجاد نمی‌کنند.
    //     ▫ Handler مربوطه در اینجا EmployeeGetByIdQueryHandler خواهد بود.
    //
    // 🔹 ساختار ارتباط:
    //     [Controller] → ارسال Query → [MediatR] → [Query Handler] → [DbContext → DTO → ResponseModel]
    //
    // 🔹 مزیت:
    //     ▫ جداسازی کامل منطق خواندن از نوشتن.
    //     ▫ سهولت در Unit Testing و Mocking (به‌ویژه با MediatR).
    //
    // 🔹 گام بعدی:
    //     فایل EmployeeGetByIdQueryHandler.cs وظیفه‌ی خواندن واقعی از پایگاه‌داده
    //     و بازگرداندن EmployeeDTO متناظر را دارد.
    //
    // 🔹 هیچ تغییر منطقی در کد داده نشده — تنها توضیحات آموزشی فارسی افزوده شدند.
    // ===========================================================================================
}
