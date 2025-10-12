using MediatR;
using Rira.Application.Common;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;

namespace Rira.Application.Features.Tasks.Commands.Create
{
    public class TaskCreateCommandHandler : IRequestHandler<TaskCreateCommand, ResponseModel<int>>
    {
        private readonly IAppDbContext _context;

        public TaskCreateCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<int>> Handle(TaskCreateCommand request, CancellationToken cancellationToken)
        {
            var entity = new TaskEntity
            {
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                Priority = request.Priority,
                DueDate = request.DueDate,
                CreatedAt = DateTime.Now.ToString("yyyy/MM/dd"),
                IsDeleted = false
            };

            _context.Tasks.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new ResponseModel<int>(true, "تسک با موفقیت ایجاد شد.", 201, entity.Id);
        }
    }
}
