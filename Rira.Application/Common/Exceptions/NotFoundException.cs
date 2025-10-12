using System;

namespace Rira.Application.Common.Exceptions
{
    /// <summary>
    /// ✅ Exception اختصاصی برای خطای «داده یافت نشد»
    /// ---------------------------------------------------------------
    /// در مواقعی که سرویس تلاش می‌کند داده‌ای با شناسهٔ مشخص را از دیتابیس دریافت کند و
    /// آن داده وجود ندارد، باید NotFoundException پرتاب شود.
    ///
    /// این کلاس باعث می‌شود API بعداً بتواند خطای 404 بسازد،
    /// و در تست‌های واحد بتوانیم رفتار عدم وجود داده را جداگانه بررسی کنیم.
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// 🔹 نوع موجودیتی که پیدا نشد (مثلاً "Task" یا "User")
        /// </summary>
        public string EntityName { get; }

        /// <summary>
        /// 🔹 شناسهٔ مورد جستجو که یافت نشد
        /// </summary>
        public object Key { get; }

        /// <summary>
        /// 🧱 سازندهٔ اصلی برای ساخت پیام مشخص خطای عدم یافتن داده.
        /// </summary>
        public NotFoundException(string entityName, object key)
            : base($"موجودیت '{entityName}' با شناسهٔ '{key}' یافت نشد.")
        {
            EntityName = entityName;
            Key = key;
        }

        /// <summary>
        /// 🧩 سازندهٔ عمومی برای پیام ساده (بدون شناسه)
        /// </summary>
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
