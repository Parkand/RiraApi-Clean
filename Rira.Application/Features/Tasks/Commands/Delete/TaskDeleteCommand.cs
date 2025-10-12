using MediatR;
using Rira.Application.Common;

namespace Rira.Application.Features.Tasks.Commands.Delete
{
    public class TaskDeleteCommand : IRequest<ResponseModel<int>>
    {
        public int Id { get; set; }
    }
}
