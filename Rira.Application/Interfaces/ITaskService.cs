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
    ///   - جداسازی منطق از پیاده‌سازی
    ///   - پشتیبانی از تزریق وابستگی (Dependency Injection)
    ///   - تست‌پذیری کامل در Unit و Integration Tests
    ///
    /// تمامی متدها خروجی‌های استاندارد از جنس ResponseModel دارند تا یکپارچگی پاسخ‌ها
    /// در سراسر پروژه Rira حفظ شود:
    ///   • Success → موفقیت‌آمیز بودن عملیات
    ///   • Message → پیام قابل‌خواندن فارسی
    ///   • Data    → داده یا شناسه خروجی مرتبط با عملیات
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
        /// 🟧 ایجاد تسک جدید — خروجی شناسه رکورد
        /// </summary>
        Task<ResponseModel<int>> CreateTaskAsync(TaskDto dto);

        /// <summary>
        /// 🟦 بروزرسانی تسک موجود — خروجی شناسه رکورد
        /// </summary>
        Task<ResponseModel<int>> UpdateTaskAsync(int id, TaskDto dto);

        /// <summary>
        /// 🟥 حذف نرم (Soft Delete) تسک — خروجی شناسه رکورد
        /// </summary>
        Task<ResponseModel<int>> DeleteTaskAsync(int id);
    }
}
