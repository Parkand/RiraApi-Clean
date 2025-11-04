using FluentValidation;

namespace Rira.Application.Features.Employees.Commands.UpdateEmployee
{
    // 📋 کلاس اعتبارسنجی فرمان ویرایش کارمند (FluentValidation + Clean Architecture)
    // -------------------------------------------------------------------------------
    // این کلاس بخشی از لایه‌ی Application است و به عنوان Validator برای EmployeeUpdateCommand
    // عمل می‌کند. هدف آن، اطمینان از درستی و کامل بودن داده‌های ورودی قبل از رسیدن به Handler است.
    //
    // Pipeline رفتار MediatR به گونه‌ای پیکربندی می‌شود که قبل از اجرای Handler،
    // تمام Validatorهای مربوط به Command مورد نظر اجرا شوند.
    //
    // این رویکرد باعث می‌شود که Handler فقط روی منطق تجاری تمرکز کند و از بررسی صحت داده‌ها جدا شود.
    public class EmployeeUpdateValidator : AbstractValidator<EmployeeUpdateCommand>
    {
        public EmployeeUpdateValidator()
        {
            // 🔹 ۱️⃣ شناسه کارمند باید معتبر باشد
            // ------------------------------------------------------------
            // جلوگیری از دریافت مقدار ۰ یا منفی برای Id.
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("شناسه کارمند باید بزرگ‌تر از صفر باشد.");

            // 🔹 ۲️⃣ بررسی محدودیت طول نام کوچک
            // ------------------------------------------------------------
            // نام اختیاری است اما در صورت ارسال، حداکثر ۵۰ کاراکتر مجاز است.
            RuleFor(x => x.FirstName)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.FirstName))
                .WithMessage("نام کارمند نمی‌تواند بیش از ۵۰ کاراکتر باشد.");

            // 🔹 ۳️⃣ بررسی محدودیت طول نام خانوادگی
            RuleFor(x => x.LastName)
                .MaximumLength(50)
                .When(x => !string.IsNullOrEmpty(x.LastName))
                .WithMessage("نام خانوادگی نمی‌تواند بیش از ۵۰ کاراکتر باشد.");

            // 🔹 ۴️⃣ فرمت شماره موبایل
            // ------------------------------------------------------------
            // فقط اعداد و دقیقاً ۱۱ رقم مجاز است.
            RuleFor(x => x.MobileNumber)
                .Matches(@"^\d{11}$")
                .When(x => !string.IsNullOrEmpty(x.MobileNumber))
                .WithMessage("شماره موبایل باید دقیقاً ۱۱ رقم باشد.");

            // 🔹 ۵️⃣ بررسی صحت فرمت ایمیل
            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("فرمت ایمیل معتبر نیست.");

            // 🔹 ۶️⃣ بررسی فرمت تاریخ تولد
            // ------------------------------------------------------------
            // تاریخ به‌صورت شمسی و با فرمت yyyy/MM/dd دریافت می‌شود.
            RuleFor(x => x.BirthDatePersian)
                .Matches(@"^\d{4}/\d{2}/\d{2}$")
                .When(x => !string.IsNullOrEmpty(x.BirthDatePersian))
                .WithMessage("فرمت تاریخ تولد باید yyyy/MM/dd باشد.");

            // 🔹 ۷️⃣ عنوان شغلی (اختیاری، حداکثر ۱۰۰ کاراکتر)
            RuleFor(x => x.JobTitle)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.JobTitle))
                .WithMessage("عنوان شغلی نمی‌تواند بیش از ۱۰۰ کاراکتر باشد.");

            // 🔹 ۸️⃣ سطح تحصیلات باید معتبر باشد (اگر ارسال شود)
            // ------------------------------------------------------------
            // Enum از نوع EducationLevelType، بنابراین عدد بزرگ‌تر از ۰ الزامی است.
            RuleFor(x => (int?)x.EducationLevel)
                .GreaterThan(0)
                .When(x => x.EducationLevel.HasValue)
                .WithMessage("سطح تحصیلات معتبر نیست.");
        }

        // ===========================================================================================
        // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
        // -------------------------------------------------------------------------------------------
        // 🔹 هر قاعده در این کلاس یک Rule محسوب می‌شود که قبل از اجرای Handler بررسی می‌گردد.
        // 🔹 استفاده از متدهای Fluent مانند:
        //     ▫ RuleFor(x => x.Field).NotEmpty() / MaximumLength() / Matches()
        //     ▫ When(...) برای اعمال قواعد شرطی.
        // 🔹 هدف اصلی: تفکیک اعتبارسنجی از منطق تجاری در معماری Clean،
        //   و جلوگیری از خطاهای زودهنگام (Fail Early Principle).
        // 🔹 در صورت نقض هر قانون، فرآیند Handle هرگز اجرا نمی‌شود،
        //   و ResponseModel شامل خطاهای اعتبارسنجی بازگردانده می‌شود.
        // 🔹 هیچ تغییری در منطق اجرایی کد ایجاد نشده — صرفاً توضیحات آموزشی اضافه گردید.
        // ===========================================================================================
    }
}
