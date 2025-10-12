using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;

namespace Rira.Application.Features.Employees.Commands.DeleteEmployee
{
    /// <summary>
    /// ⚙️ هندلر فرمان حذف کارمند از سیستم.
    /// شامل بررسی وجود کارمند و عملیات حذف امن در DbContext.
    /// </summary>
    public class EmployeeDeleteCommandHandler : IRequestHandler<EmployeeDeleteCommand, ResponseModel<int>>
    {
        private readonly IAppDbContext _context;

        public EmployeeDeleteCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseModel<int>> Handle(EmployeeDeleteCommand request, CancellationToken cancellationToken)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (employee == null)
                return ResponseModel<int>.NotFound($"❌ کارمند با شناسه {request.Id} یافت نشد.");

            try
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync(cancellationToken);

                return ResponseModel<int>.Ok(employee.Id, "✅ کارمند با موفقیت از سیستم حذف شد.");
            }
            catch (Exception ex)
            {
                return ResponseModel<int>.Fail($"خطا در حذف کارمند: {ex.Message}");
            }
        }
    }
}
