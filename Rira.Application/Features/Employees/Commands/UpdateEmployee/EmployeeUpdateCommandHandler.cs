// ===========================================================
// 📘 RiRaDocs Teaching Edition
// File: EmployeeUpdateCommandHandler.cs
// Layer: Application – CommandHandler (CQRS + MediatR)
// نسخه: RiraDocs‑v2025.11.5‑Stable‑Final‑Fixed
// ===========================================================

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rira.Application.Features.Employees.Commands.UpdateEmployee
{
    /// <summary>
    /// ⚙️ هندلر فرمان ویرایش کارمند — پیاده‌سازی استاندارد CQRS + MediatR
    /// </summary>
    public class EmployeeUpdateCommandHandler
        : IRequestHandler<EmployeeUpdateCommand, ResponseModel<int>>
    {
        // 💉 تزریق وابستگی‌ها
        private readonly IAppDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeUpdateCommandHandler(IAppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// 🎯 اجرای فرمان بروزرسانی اطلاعات کارمند
        /// </summary>
        public async Task<ResponseModel<int>> Handle(
            EmployeeUpdateCommand request,
            CancellationToken cancellationToken)
        {
            // 🔍 مرحله اول: واکشی کارمند هدف
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (employee == null)
                return ResponseModel<int>.Fail(
                    $"❌ کارمند با شناسه {request.Id} یافت نشد.", 404);

            try
            {
                // 🧩 مرحله دوم: نگاشت تغییرات
                _mapper.Map(request, employee);

                // 💾 مرحله سوم: ذخیره‌سازی در پایگاه داده
                await _context.SaveChangesAsync(cancellationToken);

                // ✅ پاسخ موفق
                return ResponseModel<int>.Ok(
                    employee.Id,
                    $"✅ کارمند با شناسه {employee.Id} با موفقیت بروزرسانی شد.");
            }
            catch (Exception ex)
            {
                // ⚠️ مدیریت خطاها
                return ResponseModel<int>.Fail(
                    $"⚠️ خطا در بروزرسانی کارمند: {ex.Message}", 500);
            }
        }

        // ===========================================================================================
        // 📚 خلاصه آموزشی (RiRaDocs)
        // -------------------------------------------------------------------------------------------
        // 🔹 این Handler منطق بروزرسانی داده‌ها را در لایه Application اجرا می‌کند.
        // 🔹 از EF Core برای مدیریت داده‌ها و AutoMapper برای نگاشت Command → Entity استفاده می‌شود.
        // 🔹 ResponseModel نوع پاسخ استاندارد پروژه است و وضعیت، پیام و داده را نگهداری می‌کند.
        // 🔹 اجرای فرمان‌ها از طریق MediatR باعث جداسازی کامل لایه‌ها و افزایش تست‌پذیری می‌شود.
        // ===========================================================================================
    }
}
