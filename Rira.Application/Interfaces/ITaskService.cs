using Rira.Application.Common;
using Rira.Application.DTOs;

namespace Rira.Application.Interfaces
{
    /// <summary>
    /// ===========================================================================
    /// 🔹 اینترفیس سرویس مربوط به عملیات مدیریتی تسک‌ها (Task)
    /// ---------------------------------------------------------------------------
    /// این اینترفیس فقط شامل امضاهای متدهایی است که در کلاس TaskService پیاده‌سازی می‌شوند.
    /// هدف:
    ///   - جدا کردن منطق از پیاده‌سازی
    ///   - پشتیبانی از تزریق وابستگی (DI)
    ///   - تست‌پذیری کامل در Unit/Integration Tests
    /// ---------------------------------------------------------------------------
    /// تمام متدها خروجی‌هایی از جنس ResponseModel دارند تا استاندارد پاسخ پروژه ریرا حفظ شود:
    ///   • Success → نشان‌دهندهٔ موفقیت عملیات
    ///   • Message → پیام فارسی برای کاربر
    ///   • Data → شیء یا لیست دادهٔ برگردانده‌شده
    /// ===========================================================================
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// 🟩 دریافت تمام تسک‌ها
        /// </summary>
        Task<ResponseModel<List<TaskDto>>> GetAllTasksAsync();

        /// <summary>
        /// 🟨 دریافت تسک بر اساس شناسه (Id)
        /// </summary>
        Task<ResponseModel<TaskDto>> GetTaskByIdAsync(int id);

        /// <summary>
        /// 🟧 ایجاد تسک جدید
        /// </summary>
        Task<ResponseModel<TaskDto>> CreateTaskAsync(TaskDto dto);

        /// <summary>
        /// 🟦 بروزرسانی تسک موجود
        /// </summary>
        Task<ResponseModel<TaskDto>> UpdateTaskAsync(int id, TaskDto dto);

        /// <summary>
        /// 🟥 حذف تسک (بر اساس شناسه)
        /// </summary>
        Task<ResponseModel<object>> DeleteTaskAsync(int id);
    }
}
