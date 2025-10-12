using FluentValidation;
using Rira.Application.DTOs;
using Rira.Domain.Entities;

namespace Rira.Application.Validators
{
    /// <summary>
    /// ✅ اعتبارسنجی DTO تسک‌ها با FluentValidation
    /// شامل بررسی رشته‌ها، Enums و طول توضیحات
    /// </summary>
    public class TaskDtoValidator : AbstractValidator<TaskDto>
    {
        public TaskDtoValidator()
        {
            // 🔹 عنوان الزامی و با طول حداقل 2 کاراکتر
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("عنوان تسک الزامی است.")
                .MinimumLength(2).WithMessage("عنوان باید حداقل ۲ کاراکتر داشته باشد.");

            // 🔹 توضیحات اختیاری اما در صورت وجود نباید خیلی کوتاه باشد
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("حداکثر طول توضیحات ۵۰۰ کاراکتر است.");

            // 🔹 وضعیت (Status) باید یکی از مقادیر Enum باشد – Case-insensitive
            RuleFor(x => x.Status)
                .Must(BeAValidStatus).WithMessage("مقدار وضعیت نامعتبر است.");

            // 🔹 اولویت (Priority) باید یکی از مقادیر Enum باشد – Case-insensitive
            RuleFor(x => x.Priority)
                .Must(BeAValidPriority).WithMessage("مقدار اولویت نامعتبر است.");

            // 🔹 تاریخ‌ها باید رشته معتبر باشند (فرمت شمسی yyyy/MM/dd)
            RuleFor(x => x.CreatedAt)
                .Must(BeAPersianDateOrNull)
                .WithMessage("تاریخ ایجاد باید به فرمت شمسی معتبر باشد.");

            RuleFor(x => x.UpdatedAt)
                .Must(BeAPersianDateOrNull)
                .WithMessage("تاریخ بروزرسانی باید به فرمت شمسی معتبر باشد.");
        }

        // 🔹 متد کمکی برای اعتبارسنجی Enum Status
        private bool BeAValidStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status)) return true;

            // بررسی معتبر بودن مقدار رشته‌ای در Enum به صورت Case-insensitive
            return Enum.TryParse(typeof(Rira.Domain.Entities.TaskStatus), status, ignoreCase: true, out _);
        }


        // متد کمکی برای اعتبارسنجی Enum Priority
        private bool BeAValidPriority(string? priority)
        {
            if (string.IsNullOrWhiteSpace(priority)) return true;
            return Enum.TryParse(typeof(TaskPriority), priority, ignoreCase: true, out _);
        }

        // متد کمکی برای بررسی فرمت تاریخ شمسی
        private bool BeAPersianDateOrNull(string? date)
        {
            if (string.IsNullOrWhiteSpace(date)) return true;
            return System.Text.RegularExpressions.Regex.IsMatch(date, @"^\d{4}/\d{2}/\d{2}$");
        }
    }
}
