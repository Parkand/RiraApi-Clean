using Rira.Domain.Enums;
using TaskStatus = Rira.Domain.Enums.TaskStatus;

namespace Rira.Application.DTOs
{
    // 🧩 مدل انتقال داده‌ها برای موجودیت Task
    // ------------------------------------------------------------
    // این کلاس برای رد و بدل داده‌ها میان کلاینت و سرور طراحی شده است.
    // وظیفه‌اش معرفی نمای بیرونی داده‌های مربوط به تسک‌ها (وظایف) در سطح API است.
    // در عملیات‌های CRUD (Create / Read / Update / Delete) از این DTO استفاده می‌شود.
    // 🔹 هیچ وابستگی مستقیمی به DomainEntity ندارد تا معماری لایه‌ای حفظ شود.
    public class TaskDto
    {
        // شناسه یکتا برای هر تسک در دیتابیس (Identity)
        // هنگام ارسال درخواست POST معمولاً مقداردهی نمی‌شود و EF Core آن را تولید می‌کند.
        public int Id { get; set; }

        // عنوان تسک — الزامی و با محدودیت ۱۵۰ کاراکتر
        // در سمت کلاینت و سرور توسط FluentValidation بررسی می‌شود.
        public string Title { get; set; } = string.Empty;

        // توضیحات اختیاری تسک — حداکثر ۵۰۰ کاراکتر
        // ممکن است کاربر آن را خالی ارسال کند.
        public string? Description { get; set; }

        // وضعیت فعلی تسک بر اساس Enum TaskStatus
        // مقادیر معتبر:
        // Pending → در انتظار، InProgress → در حال انجام،
        // Completed → انجام‌شده، Cancelled → لغوشده.
        // در Application و Mapperها به Enum از نوع TaskStatus تبدیل می‌شود.
        public TaskStatus Status { get; set; }

        // اولویت وظیفه بر اساس Enum TaskPriority
        // مقادیر معتبر: Low, Medium, High, Critical
        // کاربرد در تعیین اهمیت وظیفه در سیستم تسکینگ ریرا.
        public TaskPriority Priority { get; set; }

        // تاریخ سررسید وظیفه (DueDate)
        // 🗓 فرمت پیشنهادی: "yyyy/MM/dd" (شمسی)
        // در Validator فرمت تاریخ بررسی و تبدیل به DateTime انجام می‌شود.
        public string DueDate { get; set; } = string.Empty;

        // تاریخ ایجاد وظیفه (CreatedAt)
        // مقداردهی توسط سیستم هنگام Insert انجام می‌شود،
        // معمولاً با تابع DateHelper.GetPersianNow().
        public string? CreatedAt { get; set; }

        // تاریخ آخرین بروزرسانی (UpdatedAt)
        // هر بار عملیات Update انجام شود، این مقدار به‌روز می‌گردد.
        public string? UpdatedAt { get; set; }

        // مشخص‌کننده‌ی حذف منطقی (Soft Delete)
        // اگر مقدار true داشته باشد، وظیفه در گزارش‌ها حذف‌شده به نظر می‌رسد
        // اما همچنان در دیتابیس موجود است تا تاریخچه‌ی داده‌ها حفظ شود.
        public bool IsDeleted { get; set; } = false;

        // ===========================================================================================
        // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
        // -------------------------------------------------------------------------------------------
        // کلاس TaskDto در لایه‌ی Application مسئول جداسازی داده‌های خارجی از Domain است.
        // این کلاس داده‌های اصلی وظیفه را از نوع‌های انتزاعی (Domain Models) جدا کرده و
        // به فرمت مناسب برای ارتباط با Controller و Service تبدیل می‌کند.
        // قابلیت‌هایی مثل Soft Delete، تاریخ‌های شمسی، Enumهای وضعیت و اولویت در آن تعبیه شده‌اند.
        // استفاده هم‌زمان از FluentValidation و AutoMapper موجب کنترل کامل روی داده‌ها می‌شود.
        // هیچ تغییر اجرایی در کد ایجاد نشده — صرفاً مستندسازی آموزشی برای تیم توسعه‌ی ریرا اضافه شده است.
        // ===========================================================================================
    }
}
