using System;
using System.Collections.Generic;
using System.Linq;

namespace Rira.Application.Common.Exceptions
{
    /// <summary>
    /// ✅ Exception اختصاصی برای خطاهای اعتبارسنجی (Validation Errors)
    /// ---------------------------------------------------------------
    /// این کلاس برای زمانی استفاده می‌شود که داده‌های ورودی کاربر
    /// توسط Validatorها (مثل FluentValidation) رد شوند.
    ///
    /// هدف این Exception:
    ///   - جمع‌آوری و نگه‌داری لیست کامل خطاهای اعتبارسنجی
    ///   - ارسال پیام خوانا برای مصرف‌کننده (API / Logs)
    ///
    /// تفاوت با Exception معمولی:
    ///   - این Exception چندخطایی است (Errors لیست دارد).
    ///   - Message به‌صورت خودکار از محتوای Errors ساخته می‌شود.
    /// </summary>
    public class RiraValidationException : Exception
    {
        /// <summary>
        /// 🧩 لیست خطاهای اعتبارسنجی (مثلاً: "عنوان خالی است"، "طول کمتر از حد مجاز").
        /// </summary>
        public IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// 🧱 سازندهٔ اصلی که لیست خطاها را می‌گیرد و پیام کلی می‌سازد.
        /// </summary>
        public RiraValidationException(IEnumerable<string> errors)
            : base(GenerateMessage(errors))
        {
            Errors = errors.ToList();
        }

        /// <summary>
        /// 🔹 سازندهٔ کمکی برای وقتی فقط یک پیام ساده دارید.
        /// </summary>
        public RiraValidationException(string singleError)
            : this(new List<string> { singleError })
        {
        }

        /// <summary>
        /// ⚙️ متد داخلی برای ساخت پیام کلی خوانا از لیست خطاها.
        /// </summary>
        private static string GenerateMessage(IEnumerable<string> errors)
        {
            var joined = string.Join(" | ", errors);
            return $"Validation failed: {joined}";
        }
    }
}
