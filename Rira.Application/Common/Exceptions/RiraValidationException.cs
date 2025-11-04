using System;
using System.Collections.Generic;
using System.Linq;

namespace Rira.Application.Common.Exceptions
{
    // ✅ Exception اختصاصی برای خطاهای اعتبارسنجی (Validation Errors)
    // ---------------------------------------------------------------
    // این کلاس برای زمانی استفاده می‌شود که داده‌های ورودی کاربر
    // توسط Validatorها (مثل FluentValidation) رد شوند.
    //
    // هدف این Exception:
    //   - جمع‌آوری و نگه‌داری لیست کامل خطاهای اعتبارسنجی
    //   - ارسال پیام خوانا برای مصرف‌کننده (API / Logs)
    //
    // تفاوت با Exception معمولی:
    //   - این Exception چندخطایی است (Errors لیست دارد).
    //   - Message به‌صورت خودکار از محتوای Errors ساخته می‌شود.
    public class RiraValidationException : Exception
    {
        // 🧩 لیست خطاهای اعتبارسنجی (مثلاً: "عنوان خالی است"، "طول کمتر از حد مجاز").
        public IReadOnlyList<string> Errors { get; }

        // 🧱 سازندهٔ اصلی که لیست خطاها را می‌گیرد و پیام کلی می‌سازد.
        public RiraValidationException(IEnumerable<string> errors)
            : base(GenerateMessage(errors))
        {
            Errors = errors.ToList();
        }

        // 🔹 سازندهٔ کمکی برای وقتی فقط یک پیام ساده دارید.
        public RiraValidationException(string singleError)
            : this(new List<string> { singleError })
        {
        }

        // ⚙️ متد داخلی برای ساخت پیام کلی خوانا از لیست خطاها.
        private static string GenerateMessage(IEnumerable<string> errors)
        {
            var joined = string.Join(" | ", errors);
            return $"Validation failed: {joined}";
        }

        // ===========================================================================================
        // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
        // -------------------------------------------------------------------------------------------
        // این کلاس مخصوص نگهداری چند خطای اعتبارسنجی همزمان است
        // و در ترکیب با FluentValidation یا سرویس‌های ورودی API استفاده می‌شود.
        // Handler یا Middleware می‌تواند با خواندن لیست Errors، پاسخ‌های دقیق و خوانا تولید کند.
        // هیچ تغییر اجرایی روی کد اعمال نشده — فقط توضیحات آموزشی برای توسعه‌دهندگان ریرا اضافه شده.
        // ===========================================================================================
    }
}
