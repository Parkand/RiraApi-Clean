using Rira.Domain.Enums;
using TaskStatus = Rira.Domain.Enums.TaskStatus;

namespace Rira.Application.DTOs
{
    /// <summary>
    /// 📦 مدل انتقال داده‌ها برای موجودیت Task.
    /// این کلاس نمای بیرونی داده‌ها را بین کلاینت و سرور نشان می‌دهد.
    /// در سطح API از آن برای عملیات Create / Read / Update / Delete استفاده می‌شود.
    /// </summary>
    public class TaskDto
    {
        /// <summary>
        /// شناسه‌ی یکتا برای هر تسک.
        /// مقدار آن به‌صورت خودکار در دیتابیس (Identity) تولید می‌شود.
        /// در درخواست‌های POST نیازی به ارسال این مقدار نیست.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// عنوان تسک — الزامی و با حداکثر 150 کاراکتر.
        /// در سمت کلاینت توسط FluentValidation بررسی می‌شود.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات اختیاری تسک — حداکثر 500 کاراکتر.
        /// ممکن است کاربر آن را خالی بگذارد.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// وضعیت تسک در قالب رشته.
        /// مقادیر معتبر براساس Enum پاسکال‌کیس هستند:
        /// Pending, InProgress, Completed, Cancelled.
        /// در سمت سرور به TaskStatus تبدیل می‌شود (Case-insensitive).
        /// </summary>
        public TaskStatus Status { get; set; }

        /// <summary>
        /// اولویت تسک در قالب رشته.
        /// مقادیر معتبر براساس Enum پاسکال‌کیس هستند:
        /// Low, Medium, High, Critical.
        /// در سمت سرور به TaskPriority تبدیل می‌شود (Case-insensitive).
        /// </summary>
        public TaskPriority Priority { get; set; } 

        /// <summary>
        /// تاریخ سررسید تسک (DueDate) با فرمت شمسی "yyyy/MM/dd".
        /// توسط کاربر تعیین می‌شود و در Validator بررسی فرمت دارد.
        /// </summary>
        public string DueDate { get; set; } = string.Empty;

        /// <summary>
        /// زمان ایجاد (CreatedAt) به‌صورت تاریخ شمسی "yyyy/MM/dd".
        /// 🟢 مقدار تهی مجاز است چون از سمت سرور مقداردهی می‌شود.
        /// در زمان Insert توسط DateHelper.GetPersianNow() پر می‌گردد.
        /// </summary>
        public string? CreatedAt { get; set; }

        /// <summary>
        /// زمان آخرین بروزرسانی (UpdatedAt) به‌صورت تاریخ شمسی "yyyy/MM/dd".
        /// 🟢 مقدار تهی مجاز است چون از سمت سرور مقداردهی می‌شود.
        /// در زمان Update یا Create به‌روزرسانی می‌گردد.
        /// </summary>
        public string? UpdatedAt { get; set; }

        /// <summary>
        /// مشخص‌کننده‌ی حذف منطقی (Soft Delete).
        /// اگر true باشد، تسک در ظاهر حذف شده ولی در دیتابیس باقی است.
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}
