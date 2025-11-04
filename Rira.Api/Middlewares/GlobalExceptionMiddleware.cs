using System.Net;
using System.Text.Json;

namespace Rira.Api.Middlewares
{
    /// <summary>
    /// ⚙️ میان‌افزار (Middleware) سراسری مدیریت خطا در پروژه‌ی ریرا
    /// -----------------------------------------------------------
    /// این Middleware وظیفه دارد تمام Exceptionهای رخ‌داده در جریان اجرای درخواست‌های HTTP را کنترل کند.
    /// با استفاده از این کلاس، دیگر نیازی نیست در هر کنترلر یا سرویس، بلاک try/catch جداگانه بنویسیم.
    /// 
    /// 📋 منطق عملکرد:
    /// 1. اگر در هر بخش از لایه‌های API یا Application خطایی رخ دهد،
    ///    این میان‌افزار آن را دریافت می‌کند.
    /// 2. خطاها به صورت JSON استاندارد با فیلدهای StatusCode و Message به کلاینت بازگردانده می‌شوند.
    /// 3. در صورت نیاز می‌توان لاگ‌گیری یا ثبت در دیتابیس را در همین نقطه انجام داد.
    /// 
    /// ✅ یکی از اصول Clean Architecture این است که Exceptionها در سطح زیرساخت کنترل و از UI جدا شوند.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        /// <summary>
        /// 🧩 سازنده‌ی میان‌افزار:
        /// با تزریق وابستگی‌های RequestDelegate و ILogger از طریق DI ساخته می‌شود.
        /// RequestDelegate اشاره دارد به میان‌افزار بعدی در خط پردازش درخواست‌ها.
        /// </summary>
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// 🚦 متد اصلی Execute برای اجرای Middleware.
        /// این متد بخش «try/catch» سراسری در pipeline را اجرا می‌کند.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // عبور درخواست به میان‌افزار بعدی
                await _next(context);
            }
            catch (Exception ex)
            {
                // ثبت خطا در لاگر
                _logger.LogError(ex, "خطا در جریان پردازش درخواست رخ داد.");

                // متد مخصوص ارسال پاسخ استاندارد خطا
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// 💬 متد خصوصی برای تولید و ارسال پاسخ خطا به کلاینت
        /// در اینجا می‌توان نوع خطا را براساس Exception خاص (مثلاً ValidationException، DbUpdateException و ...) تشخیص داد.
        /// </summary>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // تعیین کد وضعیت HTTP بر اساس نوع Exception
            var statusCode = exception switch
            {
                ArgumentNullException => HttpStatusCode.BadRequest,
                KeyNotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            // ساخت پاسخ استاندارد
            var response = new
            {
                StatusCode = (int)statusCode,
                Message = exception.Message,
                Detail = "خطا در پردازش درخواست رخ داد. لطفاً با پشتیبانی تماس بگیرید."
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var options = new JsonSerializerOptions { WriteIndented = true };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }

    /// <summary>
    /// 🧩 متد Extension برای فعال‌سازی Middleware در Pipeline برنامه.
    /// این متد اجازه می‌دهد فقط با یک خط کد زیر، میان‌افزار اضافه شود:
    /// 
    /// app.UseGlobalExceptionMiddleware();
    /// </summary>
    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
