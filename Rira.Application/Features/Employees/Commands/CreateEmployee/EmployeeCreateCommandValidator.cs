using FluentValidation;

namespace Rira.Application.Features.Employees.Commands.CreateEmployee
{
    /// <summary>
    /// 🔍 اعتبارسنجی داده‌های فرمان ایجاد کارمند.
    /// وظیفه: بررسی صحت فرمت‌ها، خالی نبودن فیلدهای ضروری، و رعایت قواعد پایه‌ای.
    /// </summary>
    public class EmployeeCreateCommandValidator : AbstractValidator<EmployeeCreateCommand>
    {
        public EmployeeCreateCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("نام کارمند نباید خالی باشد.")
                .MaximumLength(50).WithMessage("نام کارمند نمی‌تواند بیشتر از ۵۰ کاراکتر باشد.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("نام خانوادگی کارمند نباید خالی باشد.")
                .MaximumLength(50).WithMessage("نام خانوادگی نمی‌تواند بیشتر از ۵۰ کاراکتر باشد.");

            RuleFor(x => x.MobileNumber)
                .NotEmpty().WithMessage("شماره موبایل اجباری است.")
                .Matches(@"^\d{11}$").WithMessage("شماره موبایل باید دقیقاً شامل ۱۱ رقم باشد.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email))
                .WithMessage("فرمت ایمیل معتبر نیست.");

            RuleFor(x => x.BirthDatePersian)
                .Matches(@"^\d{4}/\d{2}/\d{2}$")
                .WithMessage("تاریخ تولد باید به‌صورت yyyy/MM/dd وارد شود.");

            RuleFor(x => (int)x.EducationLevel)
                .GreaterThan(0).WithMessage("سطح تحصیلات معتبر نیست.");
        }
    }
}
