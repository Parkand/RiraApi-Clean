using Rira.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskStatus = Rira.Domain.Enums.TaskStatus;

namespace Rira.Domain.Entities
{
    // 🧩 کلاس دامنه‌ای TaskEntity
    // ===============================================================================
    // موجودیت (Entity) اصلی وظایف در لایه Domain.
    // در معماری Clean Architecture موجودیت‌ها نمایندگان قوانین هسته‌ای سیستم هستند.
    //
    // 🎯 هدف:
    //     ▫ نگهداری وضعیت، اولویت و تاریخ‌های مرتبط با وظیفه (Task)
    //     ▫ پشتیبانی از «حذف نرم» برای حفظ تاریخچه در دیتابیس
    //     ▫ ارائه‌ی مدل استاندارد برای نگاشت به DTO و Commands در لایه Application
    //
    // 🔹 نکته RiRaDocs:
    //     این کلاس هیچ وابستگی به EF Core یا سرویس‌ها ندارد؛
    //     DataAnnotationها فقط برای اعتبارسنجی پایه در سطح ORM اضافه شده‌اند.
    [Table("Tasks")]
    public class TaskEntity
    {
        // =====================================================================
        // 🧱 بخش ۱ — مشخصات هویتی
        // =====================================================================

        /// <summary>
        /// شناسه یکتا برای هر Task (Auto-Increment)
        /// Primary Key در جدول Tasks
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "شناسه")]
        public int Id { get; set; }

        // =====================================================================
        // 📋 بخش ۲ — اطلاعات اصلی
        // =====================================================================

        /// <summary>
        /// عنوان تسک (الزامی، حداکثر ۱۵۰ کاراکتر)
        /// نمونه: "طراحی داشبورد ادمین"
        /// </summary>
        [Required(ErrorMessage = "عنوان تسک الزامی است.")]
        [MaxLength(150, ErrorMessage = "عنوان نباید بیشتر از ۱۵۰ کاراکتر باشد.")]
        [Display(Name = "عنوان")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات متنی مکمل (اختیاری، تا ۵۰۰ کاراکتر)
        /// برای درج جزئیات کاری، نکات فنی یا یادداشت‌های داخلی.
        /// </summary>
        [MaxLength(500, ErrorMessage = "توضیحات نباید بیشتر از ۵۰۰ کاراکتر باشد.")]
        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        // =====================================================================
        // ⚙️ بخش ۳ — وضعیت و اولویت
        // =====================================================================

        /// <summary>
        /// وضعیت فعلی تسک.
        /// مقادیر: Pending | InProgress | Completed | Cancelled
        /// </summary>
        [Required(ErrorMessage = "وضعیت تسک الزامی است.")]
        [Display(Name = "وضعیت")]
        public TaskStatus Status { get; set; }

        /// <summary>
        /// سطح اولویت وظیفه.
        /// مقادیر: Low | Medium | High | Critical
        /// </summary>
        [Required(ErrorMessage = "اولویت تسک الزامی است.")]
        [Display(Name = "اولویت")]
        public TaskPriority Priority { get; set; }

        // =====================================================================
        // 📅 بخش ۴ — تاریخ‌ها (فرمت شمسی)
        // =====================================================================

        /// <summary>
        /// تاریخ سررسید وظیفه به فرمت «yyyy/MM/dd».
        /// در معماری RiRa، تمام تاریخ‌ها در قالب رشته‌ی شمسی ذخیره می‌شوند.
        /// </summary>
        [Required(ErrorMessage = "تاریخ سررسید الزامی است.")]
        [MaxLength(10, ErrorMessage = "فرمت تاریخ باید yyyy/MM/dd باشد.")]
        [Display(Name = "تاریخ سررسید")]
        public string DueDate { get; set; } = string.Empty;

        /// <summary>
        /// تاریخ ایجاد (به‌صورت رشته‌ی شمسی).
        /// معمولاً توسط DateHelper تولید و در Handlerها تنظیم می‌شود.
        /// </summary>
        [MaxLength(10, ErrorMessage = "طول تاریخ نباید بیشتر از ۱۰ کاراکتر باشد.")]
        [Display(Name = "تاریخ ایجاد")]
        public string? CreatedAt { get; set; }

        /// <summary>
        /// تاریخ آخرین ویرایش (به‌صورت رشته‌ی شمسی).
        /// هر بار به‌روزرسانی رکورد، این فیلد آپدیت می‌شود.
        /// </summary>
        [MaxLength(10, ErrorMessage = "طول تاریخ نباید بیشتر از ۱۰ کاراکتر باشد.")]
        [Display(Name = "تاریخ آخرین ویرایش")]
        public string? UpdatedAt { get; set; }

        // =====================================================================
        // 🧹 بخش ۵ — حذف نرم (Soft Delete)
        // =====================================================================

        /// <summary>
        /// نشان می‌دهد آیا تسک حذف منطقی شده است یا خیر.
        /// به‌جای حذف فیزیکی، مقدار این فیلد True می‌شود تا رکورد قابل بازیابی باشد.
        /// </summary>
        [Display(Name = "حذف شده است؟")]
        public bool IsDeleted { get; set; } = false;
    }

    // ===============================================================================
    // 📘 خلاصه آموزشی RiRaDocs Teaching Edition
    // ------------------------------------------------------------------------------
    // 🔹 مفهوم دامنه‌ای:
    //     TaskEntity هسته‌ی مدل وظایف در سیستم ریرا است.
    //     رابطه‌ی مستقیم با DTOهای Task باسلامات MappingProfile دارد
    //     و به Command و Handlerهای CQRS نگاشت می‌شود.
    //
    // 🔹 ارتباط لایه‌ها:
    //     ▫ Domain ⇒ نماینده‌ی داده‌های اصلی (Entity)
    //     ▫ Application ⇒ عملیات (Command / Query Handler / Service)
    //     ▫ Persistence ⇒ نگاشت EF Core و Repository
    //
    // 🔹 اصول طراحی RiRa:
    //     ▫ Domain Isolation — بدون وابستگی به زیرساخت.
    //     ▫ Soft Delete — حفظ تاریخچه‌ی کاری و گزارش‌پذیری.
    //     ▫ Persian Date Convention — استانداردسازی فرمت‌های تاریخی.
    //
    // 🔹 وابستگی‌ها:
    //     Rira.Domain.Enums → شامل TaskStatus و TaskPriority
    //     (در نگاشت با AutoMapper از همین Enumها استفاده می‌شود.)
    //
    // 🔹 تگ انتشار:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===============================================================================
}
