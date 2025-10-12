using System.ComponentModel.DataAnnotations;
using System;

namespace Rira.Application.DTOs
{
    /// <summary>
    /// DTO برای ایجاد یک Task جدید.
    /// شامل فیلدهای مورد نیاز برای ثبت اولیه یک وظیفه.
    /// </summary>
    public class TaskCreateDto
    {
        [Required(ErrorMessage = "عنوان (Title) الزامی است.")]
        [StringLength(100, ErrorMessage = "عنوان نمی‌تواند بیشتر از 100 کاراکتر باشد.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "توضیحات (Description) نمی‌تواند بیشتر از 500 کاراکتر باشد.")]
        public string Description { get; set; } = string.Empty;

        // تاریخ سررسید اکنون در DTO دریافت می‌شود.
        public DateTime DueDate { get; set; }

        // IsCompleted در اینجا تعریف نمی‌شود تا به صورت پیش‌فرض False تنظیم گردد.
    }
}
