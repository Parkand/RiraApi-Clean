using FluentValidation;

namespace Rira.Application.Features.Tasks.Commands.Create
{
    public class TaskCreateCommandValidator : AbstractValidator<TaskCreateCommand>
    {
        public TaskCreateCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("عنوان تسک الزامی است.")
                .MaximumLength(150).WithMessage("حداکثر طول عنوان ۱۵۰ کاراکتر است.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("توضیحات نباید بیش از ۵۰۰ کاراکتر باشد.");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("تاریخ سررسید الزامی است.")
                .Matches(@"^\d{4}/\d{2}/\d{2}$").WithMessage("فرمت تاریخ باید yyyy/MM/dd باشد.");
        }
    }
}
