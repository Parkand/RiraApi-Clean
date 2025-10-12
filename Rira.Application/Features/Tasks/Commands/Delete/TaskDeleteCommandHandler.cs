using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.Interfaces;

namespace Rira.Application.Features.Tasks.Commands.Delete
{
    public class TaskDeleteCommandHandler : IRequestHandler<TaskDeleteCommand, ResponseModel<int>>
    {
        private readonly IAppDbContext _context;

        public TaskDeleteCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<int>> Handle(TaskDeleteCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);
            if (entity == null)
                return new ResponseModel<int>(false, "تسک یافت نشد یا قبلاً حذف شده.", 404, 0);

            entity.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);

            return new ResponseModel<int>(true, "تسک با موفقیت حذف شد (Soft Delete).", 200, entity.Id);
        }
    }
}
