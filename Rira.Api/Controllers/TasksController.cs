using MediatR;
using Microsoft.AspNetCore.Mvc;
using Rira.Application.Features.Tasks.Commands.Create;
using Rira.Application.Features.Tasks.Commands.Update;
using Rira.Application.Features.Tasks.Commands.Delete;
using Rira.Application.Features.Tasks.Queries.GetAll;
using Rira.Application.Features.Tasks.Queries.GetById;
using System.Threading.Tasks;

namespace Rira.Api.Controllers
{
    /// <summary>
    /// کنترلر مدیرت عملیات تسک‌ها
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 🔹 دریافت همه تسک‌ها
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllTasks()
        {
            var result = await _mediator.Send(new TaskGetAllQuery());
            return StatusCode(result.StatusCode, result);
        }

        // 🔹 دریافت تسک بر اساس ID
        [HttpGet("get-by-id/{id:int}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var result = await _mediator.Send(new TaskGetByIdQuery { Id = id });
            return StatusCode(result.StatusCode, result);
        }

        // 🔹 ایجاد تسک جدید
        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        // 🔹 بروزرسانی تسک
        [HttpPut("update")]
        public async Task<IActionResult> UpdateTask([FromBody] TaskUpdateCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        // 🔹 حذف نرم تسک
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _mediator.Send(new TaskDeleteCommand { Id = id });
            return StatusCode(result.StatusCode, result);
        }
    }
}
