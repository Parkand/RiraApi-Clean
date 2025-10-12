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
    /// کنترلر مدیریت عملیات کارمندان در سامانه‌ی ریرا
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 🔹 دریافت همه‌ی کارمندان
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var result = await _mediator.Send(new EmployeeGetAllQuery());
            return StatusCode(result.StatusCode, result);
        }

        // 🔹 دریافت کارمند بر اساس شناسه
        [HttpGet("get-by-id/{id:int}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var result = await _mediator.Send(new EmployeeGetByIdQuery { Id = id });
            return StatusCode(result.StatusCode, result);
        }

        // 🔹 ایجاد کارمند جدید
        [HttpPost("create")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        // 🔹 بروزرسانی اطلاعات کارمند
        [HttpPut("update")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeUpdateCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        // 🔹 حذف نرم کارمند
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _mediator.Send(new EmployeeDeleteCommand { Id = id });
            return StatusCode(result.StatusCode, result);
        }
    }
}
