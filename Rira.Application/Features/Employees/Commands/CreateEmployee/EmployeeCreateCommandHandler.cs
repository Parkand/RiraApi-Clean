using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;

namespace Rira.Application.Features.Employees.Commands.CreateEmployee
{
    /// <summary>
    /// ⚙️ هندلر فرمان ایجاد کارمند جدید.
    /// وظیفه: بررسی یکتایی داده‌ها، نگاشت Command به Entity و ذخیره در پایگاه داده.
    /// </summary>
    public class EmployeeCreateCommandHandler : IRequestHandler<EmployeeCreateCommand, ResponseModel<int>>
    {
        private readonly IAppDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeCreateCommandHandler(IAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseModel<int>> Handle(EmployeeCreateCommand request, CancellationToken cancellationToken)
        {
            // بررسی تکراری بودن ایمیل یا موبایل
            bool duplicateExists = await _context.Employees
                .AnyAsync(emp =>
                    emp.Email == request.Email ||
                    emp.MobileNumber == request.MobileNumber, cancellationToken);

            if (duplicateExists)
                return ResponseModel<int>.Fail("❌ ایمیل یا شماره موبایل تکراری است.");

            try
            {
                // نگاشت Command به Entity
                var employee = _mapper.Map<EmployeeEntity>(request);

                // افزودن به دیتابیس
                await _context.Employees.AddAsync(employee, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return ResponseModel<int>.Ok(employee.Id, "✅ کارمند جدید با موفقیت ایجاد شد.");
            }
            catch (Exception ex)
            {
                return ResponseModel<int>.Fail($"خطا در ایجاد کارمند: {ex.Message}");
            }
        }
    }
}
