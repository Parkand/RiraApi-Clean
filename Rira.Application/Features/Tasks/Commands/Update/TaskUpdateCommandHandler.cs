using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.Interfaces;

namespace Rira.Application.Features.Tasks.Commands.Update
{
    public class TaskUpdateCommandHandler : IRequestHandler<TaskUpdateCommand, ResponseModel<int>>
    {
        private readonly IAppDbContext _context;

        public TaskUpdateCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<int>> Handle(TaskUpdateCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);
            if (entity == null)
                return new ResponseModel<int>(false, "تسک مورد نظر یافت نشد.", 404, 0);

            entity.Title = request.Title;
            entity.Description = request.Description;
            entity.Status = request.Status;
            entity.Priority = request.Priority;
            entity.DueDate = request.DueDate;
            entity.UpdatedAt = DateTime.Now.ToString("yyyy/MM/dd");

            await _context.SaveChangesAsync(cancellationToken);

            return new ResponseModel<int>(true, "تسک با موفقیت ویرایش شد.", 200, entity.Id);
        }
    }
}
