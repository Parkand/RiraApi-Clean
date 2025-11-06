// ===========================================================
// 📘 RiRaDocs Teaching Edition
// File: EmployeeDeleteCommandHandler.cs
// هدف: حذف کارمند با شناسه‌ی Guid بر اساس ساختار ResponseModel استاندارد
// نسخه: RiraDocs‑v2025.11.5‑FixGuidDeleteHandler
// ===========================================================

using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.Common.Exceptions;
using Rira.Application.Features.Employees.Commands.DeleteEmployee;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rira.Application.Features.Employees.Commands.DeleteEmployee
{
    /// <summary>
    /// 🧩 هندلر حذف کارمند بر اساس شناسه‌ی GUID
    /// </summary>
    public class EmployeeDeleteCommandHandler : IRequestHandler<EmployeeDeleteCommand, ResponseModel<int>>
    {
        private readonly IAppDbContext _context;

        public EmployeeDeleteCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        // ===========================================================
        // ✅ Handle
        // ===========================================================
        public async Task<ResponseModel<int>> Handle(EmployeeDeleteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 🔍 جستجوی کارمند بر اساس شناسه‌ی Guid
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

                // ⚠️ اگر کارمند پیدا نشود
                if (employee == null)
                    return ResponseModel<int>.NotFound($"کارمند با شناسه {request.Id} یافت نشد.");

                // 🗑 حذف رکورد و ذخیره‌ی تغییرات
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync(cancellationToken);

                // ✅ نتیجه موفقیت‌آمیز
                return ResponseModel<int>.Ok(1, "کارمند با موفقیت از سیستم حذف شد.");
            }
            catch (Exception ex)
            {
                // ❌ مدیریت خطاهای غیرمنتظره
                return ResponseModel<int>.Fail($"خطای حذف کارمند: {ex.Message}", 500);
            }
        }
    }
}
