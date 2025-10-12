using Rira.Application.Common;
using Rira.Application.Interfaces;

namespace Rira.Application.Base.Handler
{
    /// <summary>
    /// 🎯 کلاس پایه برای تمام Handlerهای پروژه ریرا
    /// --------------------------------------------------------------
    /// هدف:
    ///  - ایجاد نقطه مشترک برای همه Commandها و Queryها.
    ///  - فراهم کردن دسترسی مستقیم به DbContext و متدهای پاسخ استاندارد.
    ///  - کاهش تکرار کد در Handlerهای اصلی (مثل Success و Error).
    /// </summary>
    public abstract class BaseHandler
    {
        protected readonly IAppDbContext _dbContext;

        /// <summary>
        /// سازنده‌ی پایه برای تزریق وابستگی‌های مشترک
        /// </summary>
        protected BaseHandler(IAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ============================================================
        // 🔹 متدهای کمکی استاندارد برای تولید نتایج پاسخ
        // ============================================================

        protected static ResponseModel<T> Success<T>(T data, string message = "عملیات با موفقیت انجام شد") =>
            ResponseModel<T>.Ok(data, message);

        protected static ResponseModel<T> Fail<T>(string message = "خطا در اجرای عملیات") =>
            ResponseModel<T>.Fail(message);

        protected static ResponseModel<T> NotFound<T>(string message = "اطلاعات مورد نظر یافت نشد") =>
            ResponseModel<T>.NotFound(message);

        // ============================================================
        // 🔹 قابلیت مدیریت تراکنش یا عملیات پیچیده در Handler پایه
        // ============================================================

        /// <summary>
        /// ذخیره‌سازی تغییرات پایگاه داده به صورت ناهمزمان
        /// </summary>
        protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
