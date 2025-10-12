using Rira.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TaskStatus = Rira.Domain.Enums.TaskStatus;

namespace Rira.Domain.Entities
{
    /// <summary>
    /// مدل داده‌های اصلی مربوط به تسک‌ها در پروژه ریرا
    /// </summary>
    [Table("Tasks")]
    public class TaskEntity
    {
        /// <summary>
        /// شناسه یکتا (Auto-Increment)
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "شناسه")]
        public int Id { get; set; }

        /// <summary>
        /// عنوان تسک (الزامی، حداکثر ۱۵۰ کاراکتر)
        /// </summary>
        [Required(ErrorMessage = "عنوان تسک الزامی است.")]
        [MaxLength(150, ErrorMessage = "عنوان نباید بیشتر از ۱۵۰ کاراکتر باشد.")]
        [Display(Name = "عنوان")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// توضیحات مربوط به تسک (اختیاری، حداکثر ۵۰۰ کاراکتر)
        /// </summary>
        [MaxLength(500, ErrorMessage = "توضیحات نباید بیشتر از ۵۰۰ کاراکتر باشد.")]
        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        /// <summary>
        /// وضعیت فعلی تسک (Pending, InProgress, Completed, Cancelled)
        /// </summary>
        [Required(ErrorMessage = "وضعیت تسک الزامی است.")]
        [Display(Name = "وضعیت")]
        public TaskStatus Status { get; set; }

        /// <summary>
        /// سطح اولویت تسک (Low, Medium, High, Critical)
        /// </summary>
        [Required(ErrorMessage = "اولویت تسک الزامی است.")]
        [Display(Name = "اولویت")]
        public TaskPriority Priority { get; set; }

        /// <summary>
        /// تاریخ سررسید وظیفه (به‌صورت شمسی با فرمت yyyy/MM/dd)
        /// </summary>
        [Required(ErrorMessage = "تاریخ سررسید الزامی است.")]
        [MaxLength(10, ErrorMessage = "فرمت تاریخ باید yyyy/MM/dd باشد.")]
        [Display(Name = "تاریخ سررسید")]
        public string DueDate { get; set; } = string.Empty;

        /// <summary>
        /// تاریخ ایجاد تسک (به‌صورت شمسی، قابل تهی در زمان ایجاد)
        /// </summary>
        [MaxLength(10, ErrorMessage = "طول تاریخ نباید بیشتر از ۱۰ کاراکتر باشد.")]
        [Display(Name = "تاریخ ایجاد")]
        public string? CreatedAt { get; set; }

        /// <summary>
        /// تاریخ آخرین ویرایش تسک (به‌صورت شمسی، قابل تهی)
        /// </summary>
        [MaxLength(10, ErrorMessage = "طول تاریخ نباید بیشتر از ۱۰ کاراکتر باشد.")]
        [Display(Name = "تاریخ آخرین ویرایش")]
        public string? UpdatedAt { get; set; }

        /// <summary>
        /// وضعیت حذف نرم (Soft Delete)
        /// </summary>
        [Display(Name = "حذف شده است؟")]
        public bool IsDeleted { get; set; } = false;
    }

    // ---------------- Enumها ----------------

    ///// <summary>
    ///// وضعیت‌های ممکن برای یک تسک
    ///// </summary>
    //public enum TaskStatus
    //{
    //    /// <summary>در انتظار شروع</summary>
    //    Pending,

    //    /// <summary>در حال انجام</summary>
    //    InProgress,

    //    /// <summary>تکمیل شده</summary>
    //    Completed,

    //    /// <summary>لغو شده</summary>
    //    Cancelled
    //}

    ///// <summary>
    ///// سطح اولویت وظیفه
    ///// </summary>
    //public enum TaskPriority
    //{
    //    /// <summary>کم‌اهمیت</summary>
    //    Low,

    //    /// <summary>متوسط</summary>
    //    Medium,

    //    /// <summary>مهم</summary>
    //    High,

    //    /// <summary>بحرا (اضطراری)</summary>
    //    Critical
    //}
}
