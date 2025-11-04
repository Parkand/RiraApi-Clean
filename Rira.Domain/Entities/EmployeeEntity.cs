using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rira.Domain.Entities
{
    // 🧩 کلاس دامنه‌ای EmployeeEntity
    // ===============================================================================
    // این کلاس نماینده‌ی موجودیت «کارمند» در لایه دامنه (Domain Layer) پروژه‌ی RiRa است.
    // در معماری Clean Architecture، موجودیت‌ها (Entities) مستقل از لایه‌های زیرین عمل می‌کنند.
    // یعنی هیچ وابستگی به EF Core، سرویس‌ها یا DTOها ندارند.
    //
    // 🎯 اهداف طراحی:
    //     ▫ تعریف ساختار داده‌ای مورد نیاز برای کارمند.
    //     ▫ نگهداری قوانین پایه‌ای دامنه‌ای (Domain Validation).
    //     ▫ تمرکز بر معنی و رفتار موجودیت، نه نحوه‌ی ذخیره‌سازی آن.
    //
    // 🔹 نکته RiRaDocs:
    //     در Domain فقط منطق ذاتی موجودیت‌ها وجود دارد؛ قواعد پیچیده‌تر در Application مدیریت می‌شوند.
    public class EmployeeEntity
    {
        // =====================================================================
        // 🧱 بخش ۱ — هویت اصلی کارمند (Identity)
        // =====================================================================

        // شناسه یکتا (Auto Increment)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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

        // سطح تحصیلات (Enum عددی ذخیره می‌شود)
        [Required]
        public EducationLevelType EducationLevel { get; set; }

        // رشته تحصیلی یا گرایش (اختیاری)
        [MaxLength(100)]
        public string? FieldOfStudy { get; set; }

        // =====================================================================
        // 💼 بخش ۳ — وضعیت کاری و استخدامی
        // =====================================================================

        // سمت یا عنوان شغلی در سازمان
        [Required]
        [MaxLength(80)]
        public string Position { get; set; }

        // تاریخ شروع کار (به‌صورت میلادی)
        [Required]
        public DateTime HireDate { get; set; } = DateTime.Now;

        // وضعیت فعال بودن حساب کارمند
        public bool IsActive { get; set; } = true;

        // =====================================================================
        // 📧 بخش ۴ — ارتباط و اطلاعات اضافی
        // =====================================================================

        // ایمیل معتبر با فرمت استاندارد RFC
        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; }

        // توضیحات یا یادداشت داخلی (اختیاری)
        [MaxLength(500)]
        public string? Description { get; set; }

        // =====================================================================
        // 🧮 بخش ۵ — ویژگی محاسباتی (Computed Property)
        // =====================================================================

        // نام کامل کارمند (در دیتابیس ذخیره نمی‌شود)
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        // =====================================================================
        // ✅ بخش ۶ — اعتبارسنجی درون‌دامنه‌ای (Internal Validation)
        // =====================================================================
        //
        // این متد قواعد پایه Business Logic را در سطح دامنه اعمال می‌کند.
        // هیچ وابستگی به FluentValidation ندارد و صرفاً برای تضمین صحت منطقی داده‌هاست.
        public void Validate()
        {
            // الزامی بودن نام‌ها
            if (string.IsNullOrWhiteSpace(FirstName))
                throw new ArgumentException("نام کارمند نمی‌تواند خالی باشد.");

            if (string.IsNullOrWhiteSpace(LastName))
                throw new ArgumentException("نام خانوادگی کارمند نمی‌تواند خالی باشد.");

            // بررسی Enumها
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

            // الزامی بودن سمت کاری
            if (string.IsNullOrWhiteSpace(Position))
                throw new ArgumentException("سمت کاری الزامی است.");
        }

        // =====================================================================
        // 🧭 بخش ۷ — Enumهای داخلی (مقادیر محدود دامنه‌ای)
        // =====================================================================

        // 🔸 تعریف Enum جنسیت کارمند
        public enum GenderType
        {
            Male = 1,   // مرد
            Female = 2, // زن
            Other = 3   // سایر
        }

        // 🔸 تعریف Enum سطح تحصیلات استاندارد
        public enum EducationLevelType
        {
            Diploma = 1,          // دیپلم
            AssociateDegree = 2,  // کاردانی
            Bachelor = 3,         // کارشناسی
            Master = 4,           // ارشد
            Doctorate = 5,        // دکتری
            Other = 6             // سایر
        }
    }

    // ===========================================================================================
    // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
    // -------------------------------------------------------------------------------------------
    // 🔹 مفهوم:
    //     EmployeeEntity موجودیت پایه‌ی دامنه برای مدیریت اطلاعات کارمندان است.
    //     این مدل، داده‌های ساختاری (نام، تحصیلات، تماس، وضعیت) را نگه می‌دارد
    //     و قواعد اولیه‌ی اعتبارسنجی را مستقیماً در خود تعریف می‌کند.
    //
    // 🔹 نقش در Clean Architecture:
    //     ▫ استقلال از EF و سرویس‌ها (Domain Isolation)
    //     ▫ مرز بین منطق دامنه و Application
    //     ▫ نقطه ورودی برای نگاشت به DTO در AutoMapper
    //
    // 🔹 اصول رعایت‌شده:
    //     ▫ Single Responsibility — هر موجودیت فقط یک مسئولیت مفهومی دارد.
    //     ▫ Validation at Domain Level — تضمین سلامت داده‌ها پیش از عبور به لایه بالاتر.
    //     ▫ Enum Encapsulation — مقادیر محدود با نوع مشخص و خوانا.
    //
    // 🔹 ارتباط با دیگر لایه‌ها:
    //     ▫ در Application: از EmployeeDTO استفاده می‌شود.
    //     ▫ در Infrastructure (EF): این کلاس مستقیماً به DbSet<EmployeeEntity> نگاشت می‌شود.
    //
    // 🔹 تگ انتشار RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===========================================================================================
}
