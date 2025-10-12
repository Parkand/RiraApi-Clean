// نام پروژه: Rira.Application.DTOs (یا مسیری مشابه)
using System.ComponentModel.DataAnnotations;

namespace Rira.Application.DTOs
{
    /// <summary>
    /// مدل انتقال داده (DTO) برای ورودی متد PUT (به‌روزرسانی کامل یک تسک).
    /// </summary>
    public class TaskUpdateDto
    {
        // **این ویژگی (Property) برای رفع خطای شما ضروری است.**
        // باید حتماً در DTO حضور داشته باشد تا مقایسه انجام شود.
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public bool IsCompleted { get; set; }
    }
}
