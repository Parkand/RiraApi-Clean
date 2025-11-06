namespace Rira.Application.Common
{
    /// <summary>
    /// مدل پاسخ استاندارد RiRa API شامل وضعیت، پیام و داده.
    /// نسخه مستندات: RiraDocs-v2025.11.4-Stable-Final-Fixed
    /// </summary>
    // 🎯 مدل استاندارد پاسخ برای تمامی اکشن‌ها و سرویس‌های پروژه ریرا
    // -------------------------------------------------------------------
    // هدف:
    // ایجاد ساختاری یکسان برای پاسخ‌های خروجی در تمام سطوح (Controller, Handler, Service)
    // شامل وضعیت، پیام، داده و زمان ایجاد پاسخ.
    public class ResponseModel<T>
    {
        // آیا عملیات با موفقیت انجام شد؟
        public bool Success { get; set; }

        // پیغام فارسی جهت نمایش یا لاگ
        public string Message { get; set; } = string.Empty;

        // داده خروجی از نوع جنریک (مثلاً DTO یا لیست)
        public T? Data { get; set; }

        // کد وضعیت منطقی یا HTTP مرتبط (مثلاً 200، 404، 500)
        public int StatusCode { get; set; }

        // تاریخ و زمان ایجاد پاسخ (به صورت UTC)
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

        // 🔹 سازندهٔ پیش‌فرض برای مواقعی که ResponseModel به‌صورت دستی ساخته می‌شود.
        public ResponseModel() { }

        // 🔹 سازندهٔ عمومی با مشخصات کامل پاسخ (و دادهٔ اختیاری)
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

        // پاسخ موفق با داده و پیام اختیاری
        public static ResponseModel<T> Ok(T? data = default, string message = "✅ عملیات با موفقیت انجام شد") =>
            new ResponseModel<T>(true, message, 200, data);

        // پاسخ خطا با پیام و کد وضعیت دلخواه
        public static ResponseModel<T> Fail(string message = "❌ خطایی در انجام عملیات رخ داد", int statusCode = 500) =>
            new ResponseModel<T>(false, message, statusCode);

        // پاسخ در صورت عدم یافتن داده
        public static ResponseModel<T> NotFound(string message = "⚠️ داده مورد نظر یافت نشد") =>
            new ResponseModel<T>(false, message, 404);

        internal static ResponseModel<int> Ok(Guid id, string v)
        {
            throw new NotImplementedException();
        }

        // ===========================================================================================
        // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
        // -------------------------------------------------------------------------------------------
        // این کلاس الگوی اصلی پاسخ در لایه‌های مختلف سیستم ریرا است.
        // هدف آن ایجاد هماهنگی میان خروجی سرویس‌ها و کنترلرها، و سهولت در مدیریت خطاها و پیام‌ها است.
        // توسعه‌دهندگان می‌توانند در تمام Handlerها یا Serviceها صرفاً ResponseModel<T>.Ok(), Fail() و NotFound()
        // را برگردانند تا ساختار خروجی یکنواخت، تست‌پذیر و خوانا باشد.
        // هیچ تغییر اجرایی در منطق اعمال نشده — فقط توضیحات آموزشی برای تیم مستندسازی ریرا افزوده شده.
        // ===========================================================================================
    }
}
