using MediatR;
using Rira.Application.Common;
using Rira.Domain.Enums;
using TaskStatus = Rira.Domain.Enums.TaskStatus;

namespace Rira.Application.Features.Tasks.Commands.Create
{
    /// <summary>
    /// فرمان ایجاد تسک جدید بر اساس مدل داده TaskEntity
    /// </summary>
    public class TaskCreateCommand : IRequest<ResponseModel<int>>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public string DueDate { get; set; } = string.Empty;
    }
}
