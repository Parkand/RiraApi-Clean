using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.DTOs;
using Rira.Application.Interfaces;

namespace Rira.Application.Features.Tasks.Queries.GetById
{
    /// <summary>
    /// هندلر مربوط به واکشی تسک بر اساس شناسه
    /// </summary>
    public class TaskGetByIdQueryHandler : IRequestHandler<TaskGetByIdQuery, ResponseModel<TaskDto>>
    {
        private readonly IAppDbContext _context;
        private readonly IMapper _mapper;

        public TaskGetByIdQueryHandler(IAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseModel<TaskDto>> Handle(TaskGetByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

            if (entity == null)
                return new ResponseModel<TaskDto>(false, "تسک مورد نظر یافت نشد.", 404, null);

            var dto = _mapper.Map<TaskDto>(entity);
            return new ResponseModel<TaskDto>(true, "جزئیات تسک با موفقیت بازیابی شد.", 200, dto);
        }
    }
}
