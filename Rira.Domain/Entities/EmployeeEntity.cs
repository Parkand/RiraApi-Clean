using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rira.Domain.Entities
{
    /// <summary>
    /// ⚙️ کلاس دامنه‌ای «کارمند» در پروژه‌ی Rira.Api
    /// -----------------------------------------
    /// این کلاس نمایان‌گر موجودیت (Entity) کارمندان سازمان است و در لایه Domain قرار دارد.
    /// هدف:
    /// - نگهداری اطلاعات هویتی، تحصیلی، کاری و ارتباطی کارمند.
    /// - اجرای قوانین پایه‌ی اعتبارسنجی درون‌دامنه‌ای.
    /// - جداسازی منطق اصلی از لایه‌های Application و Persistence.
    ///
    /// الگو: مطابق معماری Clean Architecture ریرا
    /// یعنی بدون وابستگی به EF یا سرویس‌ها، فقط با خصوصیات و رفتار دامنه‌ای.
    /// </summary>
    public class EmployeeEntity
    {
        // =====================================================================
        // 🧩 بخش ۱ — هویت اصلی کارمند (Primary Info)
        // =====================================================================

        /// <summary>
        /// شناسه‌ی یکتا برای هر کارمند.
        /// به‌صورت عددی و Auto Increment در دیتابیس تولید می‌شود.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// نام کوچک کارمند (الزامی، حداکثر ۶۰ کاراکتر).
        /// </summary>
        [Required]
        [MaxLength(60)]
        public string FirstName { get; set; }

        /// <summary>
        /// نام خانوادگی کارمند (الزامی، حداکثر ۶۰ کاراکتر).
        /// </summary>
        [Required]
        [MaxLength(60)]
        public string LastName { get; set; }

        /// <summary>
        /// جنسیت کارمند بر اساس Enum داخلی GenderType (Male, Female, Other).
        /// </summary>
        [Required]
        public GenderType Gender { get; set; }

        /// <summary>
        /// شماره موبایل معتبر (فقط عددی، طول دقیق ۱۱ رقم؛ مثال: 09123456789).
        /// </summary>
        [Required]
        [Phone]
        [MaxLength(11)]
        public string MobileNumber { get; set; }

        /// <summary>
        /// تاریخ تولد شمسی به فرمت ثابت «yyyy/MM/dd» (مثلاً 1370/05/21).
        /// </summary>
        [MaxLength(10)]
        public string BirthDatePersian { get; set; }

        // =====================================================================
        // 🎓 بخش ۲ — اطلاعات تحصیلی کارمند
        // =====================================================================

        /// <summary>
        /// سطح تحصیلات بر اساس Enum داخلی (مثلاً لیسانس، فوق لیسانس، دکتری).
        /// مقدار عددی در دیتابیس ذخیره می‌شود ولی در منطق دامنه با Enum کار می‌کند.
        /// </summary>
        [Required]
        public EducationLevelType EducationLevel { get; set; }

        /// <summary>
        /// رشته یا گرایش تحصیلی مرتبط با حوزه کار کارمند (اختیاری).
        /// </summary>
        [MaxLength(100)]
        public string? FieldOfStudy { get; set; }

        // =====================================================================
        // 💼 بخش ۳ — اطلاعات کاری و وضعیت استخدامی
        // =====================================================================

        /// <summary>
        /// سمت یا موقعیت سازمانی کارمند (مثلاً مدیر فروش، توسعه‌دهنده، حسابدار).
        /// </summary>
        [Required]
        [MaxLength(80)]
        public string Position { get; set; }

        /// <summary>
        /// تاریخ استخدام کارمند (به‌صورت میلادی، پیش‌فرض: زمان فعلی سیستم).
        /// </summary>
        [Required]
        public DateTime HireDate { get; set; } = DateTime.Now;

        /// <summary>
        /// وضعیت فعلی کارمند در سیستم:
        /// true = فعال، false = غیرفعال یا ترک سازمان.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // =====================================================================
        // 📧 بخش ۴ — ارتباط و توضیحات اضافی
        // =====================================================================

        /// <summary>
        /// آدرس ایمیل سازمانی یا شخصی کارمند.
        /// الزاماً باید فرمت معتبر (مثلاً user@example.com) داشته باشد.
        /// </summary>
        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; }

        /// <summary>
        /// توضیحات اضافی، یادداشت داخلی یا سابقه کاری کوتاه.
        /// اختیاری، حداکثر ۵۰۰ کاراکتر.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        // =====================================================================
        // 🧮 بخش ۵ — ویژگی محاسباتی و کمکی
        // =====================================================================

        /// <summary>
        /// نام کامل کارمند = ترکیب نام و نام خانوادگی.
        /// [NotMapped]: یعنی در دیتابیس ذخیره نمی‌شود و فقط در سطح دامنه و API قابل استفاده است.
        /// </summary>
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        // =====================================================================
        // ✅ بخش ۶ — اعتبارسنجی منطقی داخلی Domain
        // =====================================================================

        /// <summary>
        /// متد Validate برای بررسی منطق درون‌دامنه‌ای.
        /// این متد بدون وابستگی به FluentValidation عمل می‌کند و فقط قوانین پایه‌ای را چک می‌کند.
        /// در لایه Application ولیدیشن فریم‌ورکی جدا انجام خواهد شد.
        /// </summary>
        public void Validate()
        {
            // الزامی بودن نام‌ها
            if (string.IsNullOrWhiteSpace(FirstName))
                throw new ArgumentException("نام کارمند نمی‌تواند خالی باشد.");

            if (string.IsNullOrWhiteSpace(LastName))
                throw new ArgumentException("نام خانوادگی کارمند نمی‌تواند خالی باشد.");

            // بررسی مقادیر Enumها
            if (Gender == 0)
                throw new ArgumentException("جنسیت کارمند باید مشخص شود.");

            if (EducationLevel == 0)
                throw new ArgumentException("سطح تحصیلات کارمند باید مشخص شود.");

            // بررسی موبایل
            if (string.IsNullOrWhiteSpace(MobileNumber))
                throw new ArgumentException("شماره موبایل الزامی است.");

            if (MobileNumber.Length != 11 || !long.TryParse(MobileNumber, out _))
                throw new ArgumentException("فرمت شماره موبایل معتبر نیست (مثلاً 09123456789).");

            // بررسی ایمیل
            if (string.IsNullOrWhiteSpace(Email))
                throw new ArgumentException("ایمیل کارمند الزامی است.");

            if (!Email.Contains("@") || !Email.Contains("."))
                throw new ArgumentException("فرمت ایمیل معتبر نیست.");

            // بررسی تاریخ تولد شمسی
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

            // سمت کاری الزامی
            if (string.IsNullOrWhiteSpace(Position))
                throw new ArgumentException("سمت کاری الزامی است.");
        }

        // =====================================================================
        // 🧭 بخش ۷ — Enumهای داخلی برای مقادیر محدود
        // =====================================================================

        /// <summary>
        /// نوع جنسیت کارمند — عددی به‌صورت int در دیتابیس ذخیره می‌شود.
        /// در منطق دامنه برای خوانایی بهتر از Enum استفاده می‌شود.
        /// </summary>
        public enum GenderType
        {
            Male = 1,   // مرد
            Female = 2, // زن
            Other = 3   // سایر یا نامشخص
        }

        /// <summary>
        /// سطح تحصیلات کارمند (EducationLevelType)
        /// جهت استانداردسازی داده‌های آموزشی و جلوگیری از ورودی‌های متنی تصادفی.
        /// </summary>
        public enum EducationLevelType
        {
            Diploma = 1,          // دیپلم
            AssociateDegree = 2,  // کاردانی
            Bachelor = 3,         // کارشناسی
            Master = 4,           // کارشناسی ارشد
            Doctorate = 5,        // دکتری
            Other = 6             // سایر یا نامشخص
        }
    }
}
