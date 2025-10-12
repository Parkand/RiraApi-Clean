using Microsoft.AspNetCore.Mvc;
using Rira.Application.DTOs;
using Rira.Application.Interfaces;
using Rira.Application.Models;

namespace Rira.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        // ======================= وابستگی‌ها ===============================
        private readonly ITaskService _taskService;

        // سازنده کنترلر با تزریق سرویس وظایف
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // ==================================================================
        // 🎯 متد ایجاد Task جدید (Create)
        // ==================================================================
        // توضیح: این اکشن دادهٔ DTO ارسالی از بدنهٔ درخواست را گرفته و آن را
        // به سرویس ارسال می‌کند تا از طریق لایه Application در دیتابیس ذخیره شود.
        // پاسخ سرویس از نوع ResponseModel<TaskDto> است که شامل Success, Message و Data خواهد بود.
        // در صورت موفقیت، پاسخ CreatedAtAction بر‌می‌گردد که محل دریافت رکورد
        // جدید (GetTaskById) را نیز مشخص می‌کند.
        // ==================================================================
        [HttpPost]
        public async Task<ActionResult<ResponseModel<TaskDto>>> CreateTask([FromBody] TaskDto dto)
        {
            // ✅ فراخوانی سرویس برای ایجاد رکورد جدید
            var result = await _taskService.CreateTaskAsync(dto);

            // بررسی موفقیت عملیات؛ در صورت شکست، پاسخ مناسب بازگردانده می‌شود
            if (!result.Success || result.Data == null)
            {
                // اگر اعتبارسنجی یا عملیات ایجاد موفق نبود، پاسخ 400 برگردان
                return BadRequest(result);
            }

            // ✅ پاسخ موفق:
            // CreatedAtAction یک پاسخ 201 برمی‌گرداند و در URL بازگشتی شناسه رکورد را قرار می‌دهد.
            // توجه: شناسه باید از داخل result.Data استخراج شود نه خود result.
            return CreatedAtAction(
                nameof(GetTaskById),               // نام اکشن مقصد برای دریافت رکورد
                new { id = result.Data!.Id },      // شناسه رکورد جدید (اصلاح‌شده)
                result                             // کل مدل پاسخ شامل داده و پیام موفقیت
            );
        }

        // ==================================================================
        // 🎯 متد واکشی تکیه بر شناسه (Read by Id)
        // ==================================================================
        // توضیح: این اکشن برای دریافت جزئیات یک تسک بر اساس شناسه استفاده می‌شود.
        // با لایه Application در ارتباط است و پاسخ استاندارد ResponseModel<TaskDto> برمی‌گرداند.
        // ==================================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<TaskDto>>> GetTaskById(int id)
        {
            // فراخوانی سرویس برای واکشی رکورد مورد نظر
            var result = await _taskService.GetTaskByIdAsync(id);

            // بررسی اینکه آیا رکورد پیدا شده یا خیر
            if (!result.Success || result.Data == null)
            {
                // اگر یافت نشده باشد، پاسخ 404 برگردان
                return NotFound(result);
            }

            // اگر موفقیت‌آمیز باشد، پاسخ 200 با دادهٔ TaskDto بازگردانده می‌شود
            return Ok(result);
        }

        // ==================================================================
        // 🎯 متد واکشی همهٔ تسک‌ها (Read All)
        // ==================================================================
        // توضیح: این اکشن فهرست تمام Taskها را از سرویس دریافت کرده و
        // با استفاده از ResponseModel<List<TaskDto>> پاسخ می‌دهد.
        // ==================================================================
        [HttpGet]
        public async Task<ActionResult<ResponseModel<List<TaskDto>>>> GetAllTasks()
        {
            var result = await _taskService.GetAllTasksAsync();
            return Ok(result);
        }

        // ==================================================================
        // 🎯 متد حذف تسک بر اساس شناسه (Delete)
        // ==================================================================
        // توضیح: این اکشن یک رکورد مشخص را بر اساس شناسه حذف می‌کند و
        // اگر حذف موفق باشد، ResponseModel با پیام "حذف موفق" برمی‌گردانده می‌شود.
        // ==================================================================
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseModel<bool>>> DeleteTask(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        // ==================================================================
        // 🎯 متد بروزرسانی تسک (Update)
        // ==================================================================
        // توضیح: این اکشن برای ویرایش رکورد موجود استفاده می‌شود.
        // اگر داده ورودی صحیح باشد و رکورد وجود داشته باشد، ResponseModel<TaskDto> بازگردانده می‌شود.
        // ==================================================================
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseModel<TaskDto>>> UpdateTask(int id, [FromBody] TaskDto dto)
        {
            var result = await _taskService.UpdateTaskAsync(id, dto);

            if (!result.Success || result.Data == null)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
