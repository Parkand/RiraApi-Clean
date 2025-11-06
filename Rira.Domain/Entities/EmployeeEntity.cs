using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rira.Domain.Entities
{
    // 🧩 کلاس دامنه‌ای EmployeeEntity
    // ===============================================================================
    // این کلاس نماینده‌ی موجودیت «کارمند» در لایه دامنه (Domain Layer) پروژه‌ی RiRa است.
    // در این نسخه (Guid Migration)، نوع شناسه از int به Guid تغییر داده شده تا با ساختار
    // کلی پروژه (TaskEntity) و سرویس‌ها سازگار گردد.
    // ===============================================================================

    public class EmployeeEntity
    {
        // =====================================================================
        // 🧱 بخش ۱ — هویت اصلی کارمند (Identity)
        // =====================================================================

        // شناسه یکتا از نوع Guid (به‌صورت خودکار مقداردهی می‌شود)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();

        // نام کوچک کارمند (الزامی، حداکثر ۶۰ کاراکتر)
        [Required]
        [MaxLength(60)]
        public string FirstName { get; set; }

        // نام خانوادگی کارمند
        [Required]
        [MaxLength(60)]
        public string LastName { get; set; }

        // جنسیت کارمند بر اساس Enum داخلی GenderType
        [Required]
        public GenderType Gender { get; set; }

        // شماره موبایل (۱۱ رقم، فقط عدد)
        [Required]
        [Phone]
        [MaxLength(11)]
        public string MobileNumber { get; set; }

        // تاریخ تولد شمسی به فرمت “yyyy/MM/dd”
        [MaxLength(10)]
        public string BirthDatePersian { get; set; }

        // =====================================================================
        // 🎓 بخش ۲ — اطلاعات تحصیلی کارمند
        // =====================================================================

        [Required]
        public EducationLevelType EducationLevel { get; set; }

        [MaxLength(100)]
        public string? FieldOfStudy { get; set; }

        // =====================================================================
        // 💼 بخش ۳ — وضعیت کاری و استخدامی
        // =====================================================================

        [Required]
        [MaxLength(80)]
        public string Position { get; set; }

        [Required]
        public DateTime HireDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // =====================================================================
        // 📧 بخش ۴ — ارتباط و اطلاعات اضافی
        // =====================================================================

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        // =====================================================================
        // 🧮 بخش ۵ — ویژگی محاسباتی (Computed Property)
        // =====================================================================
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        // =====================================================================
        // ✅ بخش ۶ — اعتبارسنجی درون‌دامنه‌ای (Internal Validation)
        // =====================================================================
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
                throw new ArgumentException("نام کارمند نمی‌تواند خالی باشد.");

            if (string.IsNullOrWhiteSpace(LastName))
                throw new ArgumentException("نام خانوادگی کارمند نمی‌تواند خالی باشد.");

            if (Gender == 0)
                throw new ArgumentException("جنسیت کارمند باید مشخص شود.");

            if (EducationLevel == 0)
                throw new ArgumentException("سطح تحصیلات کارمند باید مشخص شود.");

            if (string.IsNullOrWhiteSpace(MobileNumber))
                throw new ArgumentException("شماره موبایل الزامی است.");

            if (MobileNumber.Length != 11 || !long.TryParse(MobileNumber, out _))
                throw new ArgumentException("فرمت شماره موبایل معتبر نیست (مثلاً 09123456789).");

            if (string.IsNullOrWhiteSpace(Email))
                throw new ArgumentException("ایمیل کارمند الزامی است.");

            if (!Email.Contains("@") || !Email.Contains("."))
                throw new ArgumentException("فرمت ایمیل معتبر نیست.");

            if (!string.IsNullOrWhiteSpace(BirthDatePersian))
            {
                if (BirthDatePersian.Length != 10 || !BirthDatePersian.Contains("/"))
                    throw new ArgumentException("فرمت تاریخ تولد باید به‌صورت yyyy/MM/dd باشد.");

                var parts = BirthDatePersian.Split('/');
                if (parts.Length != 3 ||
                    parts[0].Length != 4 ||
                    parts[1].Length != 2 ||
                    parts[2].Length != 2)
                    throw new ArgumentException("فرمت تاریخ تولد معتبر نیست.");
            }

            if (string.IsNullOrWhiteSpace(Position))
                throw new ArgumentException("سمت کاری الزامی است.");
        }

        // =====================================================================
        // 🧭 بخش ۷ — Enumهای داخلی (مقادیر محدود دامنه‌ای)
        // =====================================================================

        public enum GenderType
        {
            Male = 1,
            Female = 2,
            Other = 3
        }

        public enum EducationLevelType
        {
            Diploma = 1,
            AssociateDegree = 2,
            Bachelor = 3,
            Master = 4,
            Doctorate = 5,
            Other = 6
        }
    }

    // ===========================================================================================
    // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
    // -------------------------------------------------------------------------------------------
    // 🔹 نسخه‌ی Migration:
    //     این نسخه، گذار از شناسه‌ی عددی (int) به شناسه‌ی یکتا (Guid) را در دامنه اعمال می‌کند.
    //     هدف: هماهنگی کامل بین Entity، Service، DTO و تست‌های Integration.
    //
    // 🔹 مزیت‌ها:
    //     ▫ حذف وابستگی‌های خودکار EF به Identity Sequence.
    //     ▫ ثبات در تست‌های واحد و ادغام (در فضای InMemory).
    //     ▫ یکسانی کامل با TaskEntity و سرویس‌های هم‌رده.
    //
    // 🔹 تگ انتشار RiRaDocs:
    //     RiraDocs-v2025.11.5-Stable-Guid-Migration
    // ===========================================================================================
}
