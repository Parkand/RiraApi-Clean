using System;

namespace Rira.Application.DTOs
{
    // 🧩 مدل انتقال داده برای کارمند (Employee)
    // ----------------------------------------------------
    // وظیفه:
    // این کلاس برای دریافت و ارسال داده‌ها میان API و لایهٔ Application استفاده می‌شود.
    // در واقع مدل نمایشی داده در سطح درخواست‌ها و پاسخ‌هاست.
    public class EmployeeDTO
    {
        // شناسه کارمند (Identity در دیتابیس)
        public int Id { get; set; }

        // نام
        public string FirstName { get; set; }

        // نام خانوادگی
        public string LastName { get; set; }

        // جنسیت (Enum)
        public GenderType Gender { get; set; }

        // سطح تحصیلات (Enum)
        public EducationLevelType EducationLevel { get; set; }

        // رشته تحصیلی
        public string FieldOfStudy { get; set; }

        // شماره موبایل (۱۱ رقم با صفر)
        public string MobileNumber { get; set; }

        // تاریخ تولد شمسی (yyyy/MM/dd)
        public string BirthDatePersian { get; set; }

        // سمت کاری (مثلاً Lead Developer)
        public string Position { get; set; }

        // ایمیل کاری
        public string Email { get; set; }

        // تاریخ استخدام
        public DateTime HireDate { get; set; }

        // وضعیت فعال‌بودن کارمند
        public bool IsActive { get; set; }

        // توضیحات یا یادداشت‌ها
        public string Description { get; set; }

        // نام کامل ترکیبی (خواندنی)
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

        // ===========================================================================================
        // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
        // -------------------------------------------------------------------------------------------
        // کلاس EmployeeDTO نسخه نمایشی دادهٔ کارمند در سطح Application است.
        // هدف آن جداسازی مدل‌های دامنه از مدل‌های انتقال داده برای جلوگیری از انباشت وابستگی‌هاست.
        // این کلاس در کنترلرها و Handlerها برای ارسال یا دریافت داده کارمندان استفاده می‌شود.
        // Enumهای داخلی برای جلوگیری از وابستگی مستقیم به Domain تعریف شده‌اند.
        // هیچ تغییری در اجرای کد صورت نگرفته — صرفاً مستندسازی آموزشی برای تیم توسعه ریرا افزوده شده.
        // ===========================================================================================
    }
}
