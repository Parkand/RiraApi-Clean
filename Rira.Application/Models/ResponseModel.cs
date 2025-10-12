namespace Rira.Application.Models
{
    /// <summary>
    /// مدل پاسخ استاندارد برای تمام خروجی‌های API در پروژه ریرا
    /// ساختار یکسانی برای موفقیت، پیام و داده دارد.
    /// </summary>
    public class ResponseModel<T>
    {
        /// <summary>
        /// وضعیت موفقیت عملیات (True = موفق، False = خطا)
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// پیام همراه پاسخ (اعم از پیام موفق یا متن خطا)
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// داده‌ی خروجی (می‌تواند نوع خاص DTO، Entity یا مقدار ساده باشد)
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// سازنده پیش‌فرض برای Serializing در Swagger و JSON
        /// </summary>
        public ResponseModel() { }

        /// <summary>
        /// سازنده‌ی کامل برای ساخت پاسخ سفارشی
        /// </summary>
        public ResponseModel(bool success, string message, T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}
