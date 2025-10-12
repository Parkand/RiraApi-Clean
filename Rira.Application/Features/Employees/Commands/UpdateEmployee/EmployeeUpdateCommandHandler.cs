using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;

namespace Rira.Application.Features.Employees.Commands.UpdateEmployee
{
    /// <summary>
    /// ⚙️ هندلر فرمان ویرایش کارمند.
    /// وظیفه: دریافت فرمان از MediatR، یافتن رکورد هدف و بروزرسانی فیلدهای آن.
    /// </summary>
    public class EmployeeUpdateCommandHandler : IRequestHandler<EmployeeUpdateCommand, ResponseModel<int>>
    {
        private readonly IAppDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeUpdateCommandHandler(IAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseModel<int>> Handle(EmployeeUpdateCommand request, CancellationToken cancellationToken)
        {
            // بررسی وجود کارمند
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);
            if (employee == null)
            {
                return ResponseModel<int>.NotFound($"❌ کارمند با شناسه {request.Id} یافت نشد.");
            }

            try
            {
                // نگاشت مقدارهای جدید روی مدل موجود
                _mapper.Map(request, employee);

                // ذخیره تغییرات
                await _context.SaveChangesAsync(cancellationToken);

                return ResponseModel<int>.Ok(employee.Id, $"✅ کارمند با شناسه {employee.Id} با موفقیت ویرایش شد.");
            }
            catch (Exception ex)
            {
                return ResponseModel<int>.Fail($"خطا در بروزرسانی کارمند: {ex.Message}");
            }
        }
    }
}
