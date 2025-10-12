using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.DTOs;
using Rira.Application.Interfaces;

namespace Rira.Application.Features.Tasks.Queries.GetAll
{
    public class TaskGetAllQueryHandler : IRequestHandler<TaskGetAllQuery, ResponseModel<List<TaskDto>>>
    {
        private readonly IAppDbContext _context;
        private readonly IMapper _mapper;

        public TaskGetAllQueryHandler(IAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseModel<List<TaskDto>>> Handle(TaskGetAllQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _context.Tasks
                .AsNoTracking()
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.Id)
                .ToListAsync(cancellationToken);

            var dtoList = _mapper.Map<List<TaskDto>>(tasks);
            return new ResponseModel<List<TaskDto>>(true, "لیست تسک‌ها با موفقیت بازیابی شد.", 200, dtoList);
        }
    }
}
