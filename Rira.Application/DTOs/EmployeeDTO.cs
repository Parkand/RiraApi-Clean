using System;

namespace Rira.Application.DTOs
{
    /// <summary>
    /// 🧩 مدل انتقال داده برای کارمند (Employee)
    /// ----------------------------------------------------
    /// وظیفه:
    /// این کلاس برای دریافت و ارسال داده‌ها میان API و لایهٔ Application استفاده می‌شود.
    /// در واقع مدل نمایشی داده در سطح درخواست‌ها و پاسخ‌هاست.
    /// </summary>
    public class EmployeeDTO
    {
        /// <summary>شناسه کارمند (Identity در دیتابیس)</summary>
        public int Id { get; set; }

        /// <summary>نام</summary>
        public string FirstName { get; set; }

        /// <summary>نام خانوادگی</summary>
        public string LastName { get; set; }

        /// <summary>جنسیت (Enum)</summary>
        public GenderType Gender { get; set; }

        /// <summary>سطح تحصیلات (Enum)</summary>
        public EducationLevelType EducationLevel { get; set; }

        /// <summary>رشته تحصیلی</summary>
        public string FieldOfStudy { get; set; }

        /// <summary>شماره موبایل (۱۱ رقم با صفر)</summary>
        public string MobileNumber { get; set; }

        /// <summary>تاریخ تولد شمسی (yyyy/MM/dd)</summary>
        public string BirthDatePersian { get; set; }

        /// <summary>سمت کاری (مثلاً Lead Developer)</summary>
        public string Position { get; set; }

        /// <summary>ایمیل کاری</summary>
        public string Email { get; set; }

        /// <summary>تاریخ استخدام</summary>
        public DateTime HireDate { get; set; }

        /// <summary>وضعیت فعال‌بودن کارمند</summary>
        public bool IsActive { get; set; }

        /// <summary>توضیحات یا یادداشت‌ها</summary>
        public string Description { get; set; }

        /// <summary>نام کامل ترکیبی (خواندنی)</summary>
        public string FullName => $"{FirstName} {LastName}";

        // ============================================================
        // 🔽 تعریف Enumهای داخلی مشابه Domain برای جلوگیری از وابستگی مستقیم
        // ============================================================
        public enum GenderType
        {
            Male = 1,
            Female = 2,
            Other = 3
        }

        public enum EducationLevelType
        {
            Diploma = 1,
            Associate = 2,
            Bachelor = 3,
            Master = 4,
            Doctorate = 5,
            Other = 6
        }
    }
}
