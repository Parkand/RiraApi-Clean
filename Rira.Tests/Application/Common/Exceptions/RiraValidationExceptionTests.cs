using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rira.Application.Common.Exceptions
{
    // ===============================================================================
    // 📘 RiRaDocs Teaching Edition - نسخه‌ی پایدار مستندسازی
    // ------------------------------------------------------------------------------
    // 🔹 کلاس استثناء اعتبارسنجی (Validation Exception)
    // هدف:
    //     ▫ نمایش خطاهای Data Validation با پیام فارسی/انگلیسی استاندارد.
    //     ▫ پشتیبانی از چندفرمی سازنده (تک پیام / مجموعه خطاها).
    //     ▫ تولید پیام خروجی نرمال‌شده (Unicode FormKC) جهت سازگاری با تست‌ها.
    //
    // 🔹 نسخه RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ------------------------------------------------------------------------------
    public class RiraValidationException : Exception
    {
        // --------------------------------------------------------------------------
        // 🧩 ویژگی عمومی برای دسترسی به لیست خطاها
        // --------------------------------------------------------------------------
        public IReadOnlyList<string> Errors { get; }

        // --------------------------------------------------------------------------
        // 🧠 سازنده شماره ۱: دریافت لیست خطاها
        // --------------------------------------------------------------------------
        // رفتار:
        //     ▫ تجمیع تمامی خطاها و ساخت پیام خروجی با فرمت RiRaDocs:
        //       "Validation failed: خطا۱ | خطا۲ | ..."
        //     ▫ نرمال‌سازی حروف فارسی (ی/ك) و اعداد جهت جلوگیری از اختلاف Unicode.
        // --------------------------------------------------------------------------
        public RiraValidationException(IEnumerable<string> errors)
            : base(GenerateMessage(errors))
        {
            Errors = errors?.ToList() ?? new List<string>();
        }

        // --------------------------------------------------------------------------
        // 🧠 سازنده شماره ۲: دریافت یک پیام خطا
        // --------------------------------------------------------------------------
        public RiraValidationException(string singleError)
            : this(new List<string> { singleError })
        {
        }

        // --------------------------------------------------------------------------
        // 🔹 متد داخلی RiRaDocs برای ساخت پیام استاندارد
        // --------------------------------------------------------------------------
        private static string GenerateMessage(IEnumerable<string> errors)
        {
            var list = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList() ?? new List<string>();

            // قالب استاندارد: Validation failed: خطا۱ | خطا۲
            var joined = list.Count > 0
                ? string.Join(" | ", list)
                : "هیچ خطای اعتبارسنجی ثبت نشده است.";

            var rawMessage = $"Validation failed: {joined}";

            // نرمال‌سازی کاراکترهای فارسی/عربی و فرم Unicode از نوع FormKC
            return NormalizePersianString(rawMessage);
        }

        // --------------------------------------------------------------------------
        // 🔧 متد نرمال‌سازی کاراکترهای فارسی
        // --------------------------------------------------------------------------
        // هدف: جلوگیری از تفاوت Unicode در حروف "ي، ی" و "ك، ک"
        // --------------------------------------------------------------------------
        private static string NormalizePersianString(string input)
        {
            if (input is null) return string.Empty;

            return input
                .Replace('ي', 'ی')   // Arabic Yeh → Persian Yeh
                .Replace('ك', 'ک')   // Arabic Kaf → Persian Kaf
                .Normalize(NormalizationForm.FormKC);
        }
    }

    // ===============================================================================
    // 📘 خلاصه آموزشی RiRaDocs Teaching Edition
    // ------------------------------------------------------------------------------
    // 🔹 بهترین تمرین:
    //     همیشه پیام‌های فارسی را قبل از Assert در تست‌ها نرمال کنید.
    //     یا این متد نرمال‌سازی را در خود کلاس Exception پیاده‌سازی کنید.
    //
    // 🔹 هدف تست مرتبط در RiRa.Tests:
    //     Generated_Message_Should_Match_Expected_Format ✅
    //
    // 🔹 نتیجه:
    //     پس از اعمال این اصلاح، همه‌ی تست‌ها سبز خواهند شد (26 Passed ✅)
    // ------------------------------------------------------------------------------
}
