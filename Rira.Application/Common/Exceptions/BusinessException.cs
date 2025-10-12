using System;

namespace Rira.Application.Common.Exceptions
{
    /// <summary>
    /// ✳️ Exception مخصوص خطاهای منطقی سطح کسب‌و‌کار (Business rules)
    /// زمانی استفاده می‌شود که عملیات یا داده با قوانین تجاری سازگار نباشد.
    /// </summary>
    public class BusinessException : Exception
    {
        /// <summary>
        /// 🔹 کد خطای اختیاری که برای دسته‌بندی خطاهای منطقی استفاده می‌شود.
        /// </summary>
        public string? ErrorCode { get; }

        // ======================================================================================================
        // 🔹 سازنده فقط با پیام
        // ------------------------------------------------------------------------------------------------------
        public BusinessException(string message)
            : base(message)
        {
            ErrorCode = null;
        }

        // ======================================================================================================
        // 🔹 سازنده با پیام و کد خطا
        // ------------------------------------------------------------------------------------------------------
        public BusinessException(string message, string? errorCode)
            : base(errorCode is not null && errorCode != string.Empty
                   ? $"{message} (کد خطا: {errorCode})"
                   : message)
        {
            ErrorCode = errorCode;
        }

        // ======================================================================================================
        // 🔹 سازنده با پیام، کد خطا و استثناء داخلی
        // ------------------------------------------------------------------------------------------------------
        public BusinessException(string message, string? errorCode, Exception? innerException)
            : base(errorCode is not null && errorCode != string.Empty
                   ? $"{message} (کد خطا: {errorCode})"
                   : message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
