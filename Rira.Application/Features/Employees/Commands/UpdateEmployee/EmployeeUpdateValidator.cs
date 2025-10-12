using FluentValidation;
namespace Rira.Application.Features.Employees.Commands.UpdateEmployee
{
    /// <summary>
    /// 📋 اعتبارسنجی فرمان ویرایش کارمند بر اساس مدل دامنه کامل.
    /// </summary>
    public class EmployeeUpdateValidator : AbstractValidator<EmployeeUpdateCommand>
    {
        public EmployeeUpdateValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("شناسه کارمند باید بزرگ‌تر از صفر باشد.");

            RuleFor(x => x.FirstName)
                .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.FirstName))
                .WithMessage("نام کارمند نمی‌تواند بیش از ۵۰ کاراکتر باشد.");

            RuleFor(x => x.LastName)
                .MaximumLength(50).When(x => !string.IsNullOrEmpty(x.LastName))
                .WithMessage("نام خانوادگی نمی‌تواند بیش از ۵۰ کاراکتر باشد.");

            RuleFor(x => x.MobileNumber)
                .Matches(@"^\d{11}$").When(x => !string.IsNullOrEmpty(x.MobileNumber))
                .WithMessage("شماره موبایل باید دقیقاً ۱۱ رقم باشد.");

            RuleFor(x => x.Email)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("فرمت ایمیل معتبر نیست.");

            RuleFor(x => x.BirthDatePersian)
                .Matches(@"^\d{4}/\d{2}/\d{2}$").When(x => !string.IsNullOrEmpty(x.BirthDatePersian))
                .WithMessage("فرمت تاریخ تولد باید yyyy/MM/dd باشد.");

            RuleFor(x => x.JobTitle)
                .MaximumLength(100).When(x => !string.IsNullOrEmpty(x.JobTitle))
                .WithMessage("عنوان شغلی نمی‌تواند بیش از ۱۰۰ کاراکتر باشد.");

            RuleFor(x => (int?)x.EducationLevel)
                .GreaterThan(0).When(x => x.EducationLevel.HasValue)
                .WithMessage("سطح تحصیلات معتبر نیست.");
        }
    }
}
