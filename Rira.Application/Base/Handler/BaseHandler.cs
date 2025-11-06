using Rira.Application.Common;
using Rira.Application.Interfaces;

namespace Rira.Application.Base.Handler
{
    /// <summary>
    /// 🎯 کلاس پایه برای تمام Handlerهای پروژه ریرا
    /// --------------------------------------------------------------
    /// هدف کلاس:
    ///  - ایجاد یک نقطه‌ی مشترک برای تمام Command و Query Handlerها در الگوی CQRS.
    ///  - فراهم‌سازی دسترسی به DbContext برنامه برای انجام عملیات داده‌ای.
    ///  - ارائه‌ی متدهای کمکی استاندارد جهت تولید پاسخ‌های یکپارچه (ResponseModel).
    ///   /// BaseHandler RiRa: کلاس پایه برای تمام Handlerها با تزریق خودکار IAppDbContext و IMapper.
    /// هدف: کاهش کد تکراری در Featureها.
    /// 📘 مفهوم آموزشی:
    ///  - در معماری Clean Architecture، Handlerها در لایه‌ی Application قرار دارند.
    ///  - هر Handler یک Command یا Query خاص را اجرا می‌کند و منطق را از Controller جدا می‌سازد.
    ///  - کلاس پایه `BaseHandler` این امکان را فراهم می‌کند تا کدهای تکراری مانند SaveChanges یا ساخت پاسخ استاندارد حذف شوند.
    /// </summary>
    public abstract class BaseHandler
    {
        // 💡 DbContext مستقل از EF Core اصلی؛ از طریق اینترفیس IAppDbContext تزریق می‌شود
        // تا تست‌پذیری و جداسازی وابستگی‌ها حفظ گردد.
        protected readonly IAppDbContext _dbContext;

        /// <summary>
        /// 🔹 سازنده‌ی پایه برای تزریق وابستگی‌های مشترک مانند DbContext.
        /// این سازنده در کلاس‌های ارث‌برنده فراخوانی می‌شود تا بتوانند از _dbContext استفاده کنند.
        /// </summary>
        protected BaseHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // =====================================================================
        // 🔹 متدهای کمکی استاندارد برای تولید نتایج پاسخ، جهت استفاده توسط Handlerها
        // =====================================================================

        /// <summary>
        /// ✅ ایجاد پاسخ موفقیت‌آمیز با پیام دلخواه و داده‌ی موردنظر.
        /// نحوه‌ی استفاده در Handlerها:  return Success(data, "ثبت با موفقیت انجام شد");
        /// </summary>
        protected static ResponseModel<T> Success<T>(T data, string message = "عملیات با موفقیت انجام شد") =>
            ResponseModel<T>.Ok(data, message);

        /// <summary>
        /// ❌ ایجاد پاسخ در صورت وقوع خطا در عملیات.
        /// معمولاً زمانی استفاده می‌شود که منطق اجراشده به خطا برخورد کند.
        /// </summary>
        protected static ResponseModel<T> Fail<T>(string message = "خطا در اجرای عملیات") =>
            ResponseModel<T>.Fail(message);

        /// <summary>
        /// ⚠️ تولید پاسخ در صورت عدم یافتن داده‌.
        /// نمونه کاربرد: در Queries زمانی که داده‌ی مورد نظر در دیتابیس وجود نداشته باشد.
        /// </summary>
        protected static ResponseModel<T> NotFound<T>(string message = "اطلاعات مورد نظر یافت نشد") =>
            ResponseModel<T>.NotFound(message);

        // =====================================================================
        // 🔹 قابلیت ذخیره‌سازی تراکنش‌ها یا عملیات پیچیده در Handler پایه
        // =====================================================================

        /// <summary>
        /// 💾 ذخیره‌سازی تغییرات پایگاه داده به صورت ناهمزمان (Async).
        ///  این تابع در Handlerهایی که Command اجرایی دارند فراخوانی می‌شود تا تغییرات نهایی در DbContext ثبت شود.
        ///  نکته مهم: در تست‌های واحد از طریق Mock DbContext این متد جایگزین می‌شود.
        /// </summary>
        protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
