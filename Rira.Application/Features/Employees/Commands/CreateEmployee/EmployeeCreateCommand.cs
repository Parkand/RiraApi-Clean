using MediatR;
using Rira.Application.Common;
using Rira.Application.DTOs;

namespace Rira.Application.Features.Employees.Commands.CreateEmployee
{
    /// <summary>
    /// 🎯 Command اصلی برای ثبت کارمند جدید در سیستم ریرا
    /// ---------------------------------------------------------------
    /// این کلاس یکی از بخش‌های الگوی CQS (Command Query Segregation) است،
    /// که وظیفه دارد داده‌های موردنیاز برای اجرای «عملیات افزودن کارمند»
    /// را از لایه‌ بالاتر (مثل Controller یا Service API) دریافت کند.
    ///
    /// 💡 هر Command در ریرا باید فقط مسئول *درخواست تغییر حالت سیستم* باشد،
    /// و اجرای آن توسط Handler متناظر (مثل EmployeeCreateCommandHandler) انجام می‌شود.
    ///
    /// نکات کاربردی:
    ///  - Command هیچ منطق اجرایی ندارد؛ فقط *ساختار داده ورودی* را نگه می‌دارد.
    ///  - MediatR از طریق IRequest<ResponseModel<T>> تشخیص می‌دهد که خروجی آن
    ///    باید یک ResponseModel از نوع DTO باشد.
    ///  - برای این عملیات، خروجی نهایی از جنس ResponseModel<EmployeeDTO> خواهد بود.
    /// </summary>
    public class EmployeeCreateCommand : IRequest<ResponseModel<EmployeeDTO>>
    {
        // ============================================================
        // 🔹 مشخصات پایه کارمند
        // ============================================================

        /// <summary>نام کوچک کارمند (الزامی)</summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>نام خانوادگی کارمند (الزامی)</summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>شماره موبایل (الزامی، باید ۱۱ رقم و منحصربه‌فرد باشد)</summary>
        public string MobileNumber { get; set; } = string.Empty;

        // ============================================================
        // 🔹 اطلاعات تکمیلی
        // ============================================================

        /// <summary>تاریخ تولد کارمند به صورت شمسی (مثلاً 1375/02/18)</summary>
        public string? BirthDatePersian { get; set; }

        /// <summary>آدرس ایمیل (اختیاری ولی اگر وارد شد باید معتبر باشد)</summary>
        public string? Email { get; set; }

        /// <summary>سمت یا عنوان شغلی در شرکت</summary>
        public string? Position { get; set; }

        /// <summary>وضعیت فعال یا غیرفعال بودن کارمند در سیستم</summary>
        public bool IsActive { get; set; } = true;

        // ============================================================
        // 🔹 مقادیر Enum مربوط به جنسیت و سطح تحصیلات
        // ============================================================

        /// <summary>
        /// جنسیت کارمند:
        /// 1 = Male (مرد)
        /// 2 = Female (زن)
        /// 3 = Other (سایر)
        /// مقدار عددی Enum دریافت می‌شود تا وابستگی به Domain کاهش یابد.
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// سطح تحصیلات کارمند:
        /// 1 = Diploma (دیپلم)
        /// 2 = Associate (کاردانی)
        /// 3 = Bachelor (کارشناسی)
        /// 4 = Master (کارشناسی ارشد)
        /// 5 = PhD (دکتری)
        /// 6 = Other (سایر)
        /// </summary>
        public int EducationLevel { get; set; }

        // ============================================================
        // 🔹 توضیحات و یادداشت‌ها
        // ============================================================

        /// <summary>متن توضیحات یا یادداشت آزاد درباره‌ی کارمند</summary>
        public string? Description { get; set; }

        // ============================================================
        // 🔹 نکات تکمیلی
        // ============================================================

        /*
         🔸 نکته ۱:
             این Command مستقیماً از Controller یا API فراخوانی می‌شود، مثلاً:
              var result = await _mediator.Send(new EmployeeCreateCommand { ... });

         🔸 نکته ۲:
             Handler متناظر (EmployeeCreateCommandHandler)
             در زمان اجرا داده‌های این Command را دریافت کرده،
             سپس EmployeeEntity را می‌سازد، در DbContext ذخیره می‌کند
             و در نهایت خروجی استاندارد ResponseModel<EmployeeDTO> را برمی‌گرداند.

         🔸 نکته ۳:
             تمام فیلدها فارسی‌سازی شده‌اند تا در نسخه‌های مستندات و Swagger قابل فهم باشند.
         */
    }
}
