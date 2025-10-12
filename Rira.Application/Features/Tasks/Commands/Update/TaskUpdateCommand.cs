using MediatR;
using Rira.Application.Common;
using Rira.Domain.Enums;
using TaskStatus = Rira.Domain.Enums.TaskStatus;

namespace Rira.Application.Features.Tasks.Commands.Update
{
    public class TaskUpdateCommand : IRequest<ResponseModel<int>>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public string DueDate { get; set; } = string.Empty;
    }
}
