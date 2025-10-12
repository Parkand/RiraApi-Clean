using FluentValidation;

namespace Rira.Application.Features.Tasks.Commands.Update
{
    public class TaskUpdateCommandValidator : AbstractValidator<TaskUpdateCommand>
    {
        public TaskUpdateCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("شناسه تسک معتبر نیست.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("عنوان الزامی است.")
                .MaximumLength(150);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.DueDate)
                .Matches(@"^\d{4}/\d{2}/\d{2}$").WithMessage("فرمت تاریخ باید yyyy/MM/dd باشد.");
        }
    }
}
