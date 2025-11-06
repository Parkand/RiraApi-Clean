// ===========================================================
// 📘 RiRaDocs Teaching Edition
// File: EmployeeUpdateValidator.cs
// Layer: Application – Validation (FluentValidation)
// نسخه: RiraDocs‑v2025.11.5‑Stable‑Final‑Fixed
// ===========================================================

using FluentValidation;
using Rira.Application.Features.Employees.Commands.UpdateEmployee;

namespace Rira.Application.Features.Employees.Commands.UpdateEmployee
{
    /// <summary>
    /// ✅ اعتبارسنجی مقادیر فرمان بروزرسانی کارمند (EmployeeUpdateCommand)
    /// </summary>
    public class EmployeeUpdateValidator : AbstractValidator<EmployeeUpdateCommand>
    {
        public EmployeeUpdateValidator()
        {
            // 🆔 شناسه باید مقدار معتبر Guid داشته باشد
            RuleFor(e => e.Id)
                .NotEmpty().WithMessage("شناسه کارمند نمی‌تواند خالی باشد.");

            // 🧍‍♂️ نام و نام خانوادگی
            RuleFor(e => e.FirstName)
                .NotEmpty().WithMessage("نام الزامی است.")
                .MaximumLength(100).WithMessage("نام نمی‌تواند بیش از ۱۰۰ کاراکتر باشد.");

            RuleFor(e => e.LastName)
                .NotEmpty().WithMessage("نام خانوادگی الزامی است.")
                .MaximumLength(100).WithMessage("نام خانوادگی نمی‌تواند بیش از ۱۰۰ کاراکتر باشد.");

            // 📧 ایمیل
            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("ایمیل الزامی است.")
                .EmailAddress().WithMessage("فرمت ایمیل معتبر نیست.")
                .MaximumLength(200).WithMessage("ایمیل نمی‌تواند بیش از ۲۰۰ کاراکتر باشد.");

            // 📱 شماره موبایل (الگوی ساده‌ی موبایل ایران)
            RuleFor(e => e.MobileNumber)
                .NotEmpty().WithMessage("شماره موبایل الزامی است.")
                .Matches(@"^(?:0|\+98)?9\d{9}$").WithMessage("شماره موبایل معتبر نیست.");

            // 💼 سمت شغلی
            RuleFor(e => e.Position)
                .NotEmpty().WithMessage("سمت شغلی الزامی است.")
                .MaximumLength(100).WithMessage("سمت شغلی نمی‌تواند بیش از ۱۰۰ کاراکتر باشد.");

            // 📜 توضیحات اختیاری، ولی حداکثر ۵۰۰ کاراکتر
            RuleFor(e => e.Description)
                .MaximumLength(500).WithMessage("توضیحات نمی‌تواند بیش از ۵۰۰ کاراکتر باشد.");
        }

        // ===========================================================================================
        // 📚 یادداشت آموزشی (RiRaDocs)
        // -------------------------------------------------------------------------------------------
        // 🔹 این Validator قب فرمان به Handler اجرا می‌شود.
        // 🔹 در صور نقض قوانین، FluentValidation یک ValidationException تولید می‌کند.
        // 🔹 پیغام‌های خطا به صورت فارس و کاربرپسند طراحی شده‌اند.
        // 🔹 قرار است د فاز آتی فریم‌ورک تست (FluentAssertions) برای آزمون اعتبارسنجی‌ها استفاده شود.
        // ===========================================================================================
    }
}
