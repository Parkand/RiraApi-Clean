using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rira.Application.Features.Employees.Commands.CreateEmployee;
using Rira.Application.Features.Employees.Commands.DeleteEmployee;
using Rira.Application.Features.Employees.Commands.UpdateEmployee;
using Rira.Application.Features.Employees.Queries.GetAllEmployees;
using Rira.Application.Features.Employees.Queries.GetById;

namespace Rira.Api.Controllers
{
    /// <summary>
    /// 🧭 کنترلر مدیریت عملیات "کارمندان" در سامانه‌ی ریرا
    /// -------------------------------------------------------
    /// این کنترلر بخشی از لایه‌ی API است که وظیفه‌ی دریافت و پردازش
    /// درخواست‌های HTTP مرتبط با موجودیت Employee را برعهده دارد.
    ///
    /// ✅ این کنترلر از الگوی CQRS (Command–Query Responsibility Segregation)
    /// پیروی می‌کند، یعنی:
    ///   - درخواست‌های "خواندن" (Query) و "نوشتن" (Command) از هم جدا هستند.
    /// ✅ از کتابخانه‌ی MediatR استفاده می‌شود تا هر درخواست به Handler مربوطه
    /// در لایه‌ی Application ارسال شود، بدون نیاز به منطق تجاری در Controller.
    ///
    /// در نتیجه Controller نقش هماهنگ‌کننده‌ی سبک دارد (Thin Controller),
    /// تمام پردازش‌ها در Handlerهای لایه‌ی Application انجام می‌گیرد.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// 🧩 سازنده کنترلر که از طریق DI، واسط MediatR را دریافت می‌کند.
        /// MediatR مسئول ارسال Commandها یا Queryها به Handlerهای مربوط است.
        /// </summary>
        public EmployeesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ================================================================
        // 🔹 دریافت همه‌ی کارمندان
        // ================================================================

        /// <summary>
        /// متد HTTP GET برای دریافت فهرست کامل کارمندان.
        /// این متد از Query مخصوص گرفتن همه‌ی کارمندان استفاده می‌کند.
        /// </summary>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllEmployees()
        {
            // ارسال درخواست به Handler مربوطه در Application
            var result = await _mediator.Send(new EmployeeGetAllQuery());

            // پاسخ به کلاینت بر اساس StatusCode برگردانده‌شده از Handler
            return StatusCode(result.StatusCode, result);
        }

        // ================================================================
        // 🔹 دریافت کارمند بر اساس شناسه (ID)
        // ================================================================

        /// <summary>
        /// متد HTTP GET برای دریافت جزئیات یک کارمند خاص بر اساس شناسه.
        /// Query نوع <see cref="EmployeeGetByIdQuery"/> از لایه‌ی Application استفاده می‌شود.
        /// مسیر درخواست: GET  /api/employees/get-by-id/{id}
        /// </summary>
        [HttpGet("get-by-id/{id:int}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var result = await _mediator.Send(new EmployeeGetByIdQuery { Id = id });
            return StatusCode(result.StatusCode, result);
        }

        // ================================================================
        // 🔹 ایجاد کارمند جدید
        // ================================================================

        /// <summary>
        /// متد HTTP POST برای افزودن یک کارمند جدید.
        /// داده‌ی ورودی از بدنه‌ی درخواست (JSON Body) دریافت می‌شود.
        /// ولیدیشن مدل توسط FluentValidation در لایه‌ی Application انجام می‌شود.
        /// Command نوع <see cref="EmployeeCreateCommand"/> فراخوانی می‌شود.
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateCommand command)
        {
            // اعتبارسنجی مدل ورودی براساس Annotationها یا FluentValidator
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ارسال Command برای ساخت کارمند جدید
            var result = await _mediator.Send(command);

            // پاسخ استاندارد بر اساس خروجی Handler
            return StatusCode(result.StatusCode, result);
        }

        // ================================================================
        // 🔹 بروزرسانی اطلاعات کارمند
        // ================================================================

        /// <summary>
        /// متد HTTP PUT جهت بروزرسانی اطلاعات یک کارمند موجود.
        /// Command نوع <see cref="EmployeeUpdateCommand"/> برای اجرای عملیات Update استفاده می‌شود.
        /// مسیر: PUT /api/employees/update
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeUpdateCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        // ================================================================
        // 🔹 حذف نرم (Soft Delete) کارمند
        // ================================================================

        /// <summary>
        /// متد HTTP DELETE برای حذف نرم (غیرفعال‌سازی) یک کارمند بر اساس شناسه.
        /// Command نوع <see cref="EmployeeDeleteCommand"/> اجرا می‌شود.
        /// حذف نرم یعنی رکورد در پایگاه داده باقی می‌ماند ولی وضعیت آن “غیرفعال” می‌شود.
        /// </summary>
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _mediator.Send(new EmployeeDeleteCommand { Id = id });
            return StatusCode(result.StatusCode, result);
        }
    }
}
