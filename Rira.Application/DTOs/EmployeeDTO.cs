using System;

namespace Rira.Application.DTOs
{
    // ===========================================================
    // 🧩 RiRaDocs Teaching Edition – Data Transfer Layer
    // ===========================================================
    // File: EmployeeDTO.cs
    // Description:
    // مدل انتقال داده (DTO) برای موجودیت کارمند.
    // پس از مهاجرت نوع شناسه از int به Guid، این مدل نیز هماهنگ‌سازی شده
    // تا با EmployeeEntity و EmployeeService در ساختار Clean Architecture مطابقت داشته باشد.
    // ===========================================================

    public class EmployeeDTO
    {
        // =====================================================================
        // 🧱 Identity
        // =====================================================================

        /// <summary>
        /// شناسه یکتای کارمند (Guid)
        /// </summary>
        public Guid Id { get; set; }

        // =====================================================================
        // 👤 مشخصات فردی
        // =====================================================================

        public string FirstName { get; set; }
        public string LastName { get; set; }

        /// <summary>
        /// جنسیت کارمند (Enum داخلی)
        /// </summary>
        public GenderType Gender { get; set; }

        /// <summary>
        /// تاریخ تولد شمسی به فرمت yyyy/MM/dd
        /// </summary>
        public string BirthDatePersian { get; set; }

        // =====================================================================
        // 🎓 اطلاعات تحصیلی
        // =====================================================================

        public EducationLevelType EducationLevel { get; set; }
        public string? FieldOfStudy { get; set; }

        // =====================================================================
        // 💼 اطلاعات شغلی
        // =====================================================================

        public string Position { get; set; }
        public DateTime HireDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // =====================================================================
        // ☎️ ارتباطات
        // =====================================================================

        public string MobileNumber { get; set; }
        public string Email { get; set; }

        // =====================================================================
        // 🗒️ موارد اضافی
        // =====================================================================

        public string? Description { get; set; }

        // نام ترکیبی فقط برای خواندن
        public string FullName => $"{FirstName} {LastName}";

        // =====================================================================
        // 🔽 Enumهای داخلی (ایزوله از Domain برای کاهش وابستگی)
        // =====================================================================

        public enum GenderType
        {
            Male = 1,   // مرد
            Female = 2, // زن
            Other = 3   // سایر
        }

        public enum EducationLevelType
        {
            Diploma = 1,          // دیپلم
            AssociateDegree = 2,  // کاردانی
            Bachelor = 3,         // کارشناسی
            Master = 4,           // کارشناسی ارشد
            Doctorate = 5,        // دکتری
            Other = 6             // سایر
        }

        // =====================================================================
        // 📘 خلاصهٔ آموزشی (RiRaDocs Teaching Edition)
        // ---------------------------------------------------------------------
        // 🔹 هدف:
        //   ارائه مدل دادهٔ سبک برای انتقال بین Controller ↔ Application Layer.
        //
        // 🔹 تغییرات نسخهٔ Guid Migration:
        //   ▫ نوع Id از int به Guid تغییر یافت.
        //   ▫ تطبیق کامل با EmployeeEntity و MapperProfile جدید.
        //
        // 🔹 نقش در معماری Clean Architecture:
        //   ▫ DTO مستقل از Domain و Infrastructure است.
        //   ▫ ابزاری برای تبادل امن داده در handlerها و سرویس‌ها.
        //
        // 🔹 تگ انتشار RiRaDocs:
        //   RiraDocs‑v2025.11.5‑Stable‑Guid‑Migration
        // =====================================================================
    }
}
