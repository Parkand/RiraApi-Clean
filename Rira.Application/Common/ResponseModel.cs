namespace Rira.Application.Common
{
    /// <summary>
    /// 🎯 مدل استاندارد پاسخ برای تمامی اکشن‌ها و سرویس‌های پروژه ریرا
    /// هدف:
    /// ایجاد ساختاری یکسان برای پاسخ‌های خروجی در تمام سطوح (Controller, Handler, Service)
    /// شامل وضعیت، پیام، داده و زمان ایجاد پاسخ.
    /// </summary>
    public class ResponseModel<T>
    {
        /// <summary>آیا عملیات با موفقیت انجام شد؟</summary>
        public bool Success { get; set; }

        /// <summary>پیغام فارسی جهت نمایش یا لاگ</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>داده خروجی از نوع جنریک (مثلاً DTO یا لیست)</summary>
        public T? Data { get; set; }

        /// <summary>کد وضعیت منطقی یا HTTP مرتبط (مثلاً 200، 404، 500)</summary>
        public int StatusCode { get; set; }

        /// <summary>تاریخ و زمان ایجاد پاسخ (به صورت UTC)</summary>
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

        public ResponseModel() { }

        public ResponseModel(bool success, string message, int statusCode, T? data = default)
        {
            Success = success;
            Message = message;
            StatusCode = statusCode;
            Data = data;
        }

        // ============================================================
        // 🔹 متدهای کمکی استاندارد برای پاسخ‌های رایج
        // ============================================================

        /// <summary>پاسخ موفق با داده و پیام اختیاری</summary>
        public static ResponseModel<T> Ok(T? data = default, string message = "✅ عملیات با موفقیت انجام شد") =>
            new ResponseModel<T>(true, message, 200, data);

        /// <summary>پاسخ خطا با پیام و کد وضعیت دلخواه</summary>
        public static ResponseModel<T> Fail(string message = "❌ خطایی در انجام عملیات رخ داد", int statusCode = 500) =>
            new ResponseModel<T>(false, message, statusCode);

        /// <summary>پاسخ در صورت عدم یافتن داده</summary>
        public static ResponseModel<T> NotFound(string message = "⚠️ داده مورد نظر یافت نشد") =>
            new ResponseModel<T>(false, message, 404);
    }
}
