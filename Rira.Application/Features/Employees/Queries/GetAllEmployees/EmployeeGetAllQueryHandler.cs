using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.DTOs;
using Rira.Application.Base.Handler;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rira.Application.Features.Employees.Queries.GetAllEmployees
{
    /// <summary>
    /// 🎯 QueryHandler مربوط به EmployeeGetAllQuery
    /// -------------------------------------------------------------
    /// این کلاس از الگوی MediatR پیروی می‌کند و وظیفه دارد
    /// منطق واکشی لیست کارمندان را از پایگاه داده اجرا کرده،
    /// سپس با کمک AutoMapper خروجی را به DTO تبدیل کند.
    ///
    /// 🔹 این Handler از BaseHandler ارث‌بری می‌کند تا بتواند از
    ///  متدهای کمکی مانند Success(), Fail(), NotFound() استفاه کند.
    ///
    /// 🔸 خروجی نهایی: ResponseModel<List<EmployeeDTO>>
    /// </summary>
    public class EmployeeGetAllQueryHandler
        : BaseHandler, IRequestHandler<EmployeeGetAllQuery, ResponseModel<List<EmployeeDTO>>>
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// سازنده‌ی اصلی که وابستگی‌های موردنیاز را تزریق می‌کند.
        /// </summary>
        public EmployeeGetAllQueryHandler(IAppDbContext dbContext, IMapper mapper)
            : base(dbContext)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// ⚙️ متد اصلی برای اجرای Query — واکشی همه‌ی کارمندان
        /// </summary>
        public async Task<ResponseModel<List<EmployeeDTO>>> Handle(
            EmployeeGetAllQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // واکشی تمام رکوردهای کارمند از DbSet
                var employees = await _dbContext.Employees
                    .AsNoTracking()
                    .OrderByDescending(e => e.Id)
                    .ToListAsync(cancellationToken);

                // در صورت نبود هیچ کارمند، پیام مناسب بازگردانده شود
                if (employees == null || employees.Count == 0)
                    return NotFound<List<EmployeeDTO>>("هیچ کارمندی در سیستم ثبت نشده است.");

                // تبدیل به DTO با استفاده از Mapper
                var mappedDtos = _mapper.Map<List<EmployeeDTO>>(employees);

                // بازگشت پاسخ موفق استاندارد
                return Success(mappedDtos, "لیست تمام کارمندان با موفقیت بازیابی شد.");
            }
            catch (Exception ex)
            {
                // مدیریت خطا با پاسخ Fail استاندارد
                return Fail<List<EmployeeDTO>>($"خطا در دریافت لیست کارمندان: {ex.Message}");
            }
        }
    }
}
