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
    /// 🎯 کنترلر مدیریت تراکنش‌ها و عملیات‌های موجودیت تسک (Task)
    /// -------------------------------------------------------------
    /// این کنترلر یکی از نقاط ورودی اصلی در لایه‌ی API ریرا است که عملیات CRUD مربوط به وظایف (Tasks)
    /// را مدیریت می‌کند. 
    /// معماری به‌کاررفته در این بخش مبتنی بر الگوی **CQRS** و کتابخانه‌ی **MediatR** است.
    /// 
    /// در این الگو، کنترلر هیچ منطق تجاری (Business Logic) را اجرا نمی‌کند؛
    /// بلکه فقط درخواست را دریافت و از طریق MediatR به Handler (دست‌کار) مربوطه در لایه‌ی Application ارسال می‌کند.
    /// 
    /// مزیت: کاهش وابستگی، تست‌پذیری بالاتر و استقلال کامل بین لایه‌ها.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// 🎬 سازنده‌ی کلاس که IMediator را از طریق Dependency Injection تزریق می‌کند.
        /// </summary>
        /// <param name="mediator">واسط ارتباط بین لایه‌ی API و Handlerهای CQRS</param>
        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ===============================================================
        // 🔹 دریافت تمام تسک‌ها
        // ===============================================================

        /// <summary>
        /// متد HTTP GET برای دریافت فهرست تمامی وظایف موجود در سامانه.
        /// از Query نوع <see cref="TaskGetAllQuery"/> جهت بازیابی داده‌ها از Application استفاده می‌شود.
        /// مسیر نمونه: GET /api/tasks/get-all
        /// </summary>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllTasks()
        {
            var result = await _mediator.Send(new TaskGetAllQuery());
            return StatusCode(result.StatusCode, result);
        }

        // ===============================================================
        // 🔹 دریافت تسک بر اساس شناسه
        // ===============================================================

        /// <summary>
        /// متد HTTP GET برای دریافت جزئیات یک وظیفه خاص بر اساس شناسه (ID).
        /// به‌کمک Query نوع <see cref="TaskGetByIdQuery"/> اجرا می‌شود.
        /// مسیر نمونه: GET /api/tasks/get-by-id/5
        /// </summary>
        [HttpGet("get-by-id/{id:int}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var result = await _mediator.Send(new TaskGetByIdQuery { Id = id });
            return StatusCode(result.StatusCode, result);
        }

        // ===============================================================
        // 🔹 ایجاد تسک جدید
        // ===============================================================

        /// <summary>
        /// متد HTTP POST جهت ایجاد وظیفه جدید در سیستم.
        /// داده‌های ورودی از طریق بدنه‌ی درخواست (Body) به‌صورت JSON ارسال می‌شوند.
        /// Handler مربوطه در Application کلاس <see cref="TaskCreateCommand"/> است.
        /// مسیر نمونه: POST /api/tasks/create
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        // ===============================================================
        // 🔹 بروزرسانی اطلاعات تسک
        // ===============================================================

        /// <summary>
        /// متد HTTP PUT جهت بروزرسانی اطلاعات یکی از وظایف موجود.
        /// Command نوع <see cref="TaskUpdateCommand"/> داده‌های جدید را به Handler ارسال می‌کند.
        /// مسیر نمونه: PUT /api/tasks/update
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateTask([FromBody] TaskUpdateCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);
            return StatusCode(result.StatusCode, result);
        }

        // ===============================================================
        // 🔹 حذف نرم (Soft Delete)
        // ===============================================================

        /// <summary>
        /// متد HTTP DELETE برای حذف نرم وظیفه، به این معنی که رکورد از دیتابیس حذف نمی‌شود بلکه وضعیت آن به Deleted تغییر می‌یابد.
        /// Command متناظر: <see cref="TaskDeleteCommand"/>.
        /// مسیر نمونه: DELETE /api/tasks/delete/5
        /// </summary>
        [HttpDelete("delete/{id:int}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _mediator.Send(new TaskDeleteCommand { Id = id });
            return StatusCode(result.StatusCode, result);
        }
    }
}
