using FluentValidation;
using Rira.Application.DTOs;
using Rira.Domain.Enums;
using System.Text.RegularExpressions;
using TaskStatus = Rira.Domain.Enums.TaskStatus;

namespace Rira.Application.Validators
{
    /// <summary>
    /// ✅ اعتبارسنجی DTO مربوط به تسک‌ها با FluentValidation
    /// شامل بررسی الزامات رشته‌ای، Enumها و قالب تاریخ شمسی
    /// </summary>
    public class TaskDtoValidator : AbstractValidator<TaskDto>
    {
        public TaskDtoValidator()
        {
            // 🔹 عنوان الزامی و حداقل ۲ کاراکتر
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("عنوان تسک الزامی است.")
                .MinimumLength(2).WithMessage("عنوان باید حداقل ۲ کاراکتر داشته باشد.");

            // 🔹 توضیحات اختیاری با حداکثر ۵۰۰ کاراکتر
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("حداکثر طول توضیحات ۵۰۰ کاراکتر است.");

            // 🔹 وضعیت (Status) باید یکی از مقادیر معتبر Enum باشد
            RuleFor(x => x.Status)
                .Must(BeAValidStatus)
                .WithMessage("مقدار وضعیت نامعتبر است.");

            // 🔹 اولویت (Priority) باید یکی از مقادیر معتبر Enum باشد
            RuleFor(x => x.Priority)
                .Must(BeAValidPriority)
                .WithMessage("مقدار اولویت نامعتبر است.");

            // 🔹 تاریخ‌های CreatedAt و UpdatedAt باید به فرمت شمسی (yyyy/MM/dd) باشند
            RuleFor(x => x.CreatedAt)
                .Must(BeAPersianDateOrNull)
                .WithMessage("تاریخ ایجاد باید به فرمت شمسی معتبر باشد.");

            RuleFor(x => x.UpdatedAt)
                .Must(BeAPersianDateOrNull)
                .WithMessage("تاریخ بروزرسانی باید به فرمت شمسی معتبر باشد.");
        }

        // 🔹 بررسی معتبر بودن مقدار Enum وضعیت
        private bool BeAValidStatus(TaskStatus status)
        {
            return Enum.IsDefined(typeof(TaskStatus), status);
        }

        // 🔹 بررسی معتبر بودن مقدار Enum اولویت
        private bool BeAValidPriority(TaskPriority priority)
        {
            return Enum.IsDefined(typeof(TaskPriority), priority);
        }

        // 🔹 بررسی معتبر بودن فرمت تاریخ شمسی یا نال بودن
        private bool BeAPersianDateOrNull(string? date)
        {
            if (string.IsNullOrWhiteSpace(date)) return true;
            return Regex.IsMatch(date, @"^\d{4}/\d{2}/\d{2}$");
        }
    }
}
