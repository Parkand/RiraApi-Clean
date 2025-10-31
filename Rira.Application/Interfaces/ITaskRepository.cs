using System.Collections.Generic;
using System.Threading.Tasks;
using Rira.Domain.Entities;

namespace Rira.Application.Interfaces
{
    /// <summary>
    /// ⛓️ رابط اصلی ریپازیتوری تسک‌ها در معماری Clean ریرا
    /// -------------------------------------------------------
    /// این اینترفیس وظیفهٔ تعریف عملیات CRUD سطح پایگاه‌داده را بر عهده دارد.
    /// در این لایه، هیچ‌گونه لاجیک تجاری (Business Logic) نباید وجود داشته باشد.
    /// کلیه‌ی عملیات‌ها باید **Async** باشند تا مطابق استانداردهای جدید .NET انجام شوند.
    /// 
    /// 🧩 نکات کلیدی:
    /// - متدهای Create و Update مقدار **ID رکورد** ایجاد/ویرایش‌شده را برمی‌گردانند (Task<int>)
    /// - متدهای Get فقط داده را واکشی می‌کنند.
    /// - SoftDelete باید در سطح Service انجام گیرد و سپس UpdateAsync صدا زده شود.
    /// - پیاده‌سازی این رابط در لایه‌ی Persistence (EFCore) انجام می‌شود.
    /// </summary>
    public interface ITaskRepository
    {
        /// <summary>
        /// 🟢 ایجاد رکورد جدید تسک
        /// </summary>
        /// <param name="entity">موجودیت تسک برای درج</param>
        /// <returns>
        /// عدد شناسه‌ی درج‌شده (ID > 0 در صورت موفقیت)
        /// </returns>
        Task<int> CreateAsync(TaskEntity entity);

        /// <summary>
        /// 🟡 ویرایش رکورد موجود
        /// </summary>
        /// <param name="entity">موجودیت به‌روزشده تسک</param>
        /// <returns>
        /// عدد شناسه‌ی رکورد ویرایش‌شده (ID > 0 در صورت موفقیت)
        /// </returns>
        Task<int> UpdateAsync(TaskEntity entity);

        /// <summary>
        /// 🔵 دریافت تسک بر اساس شناسه
        /// </summary>
        /// <param name="id">شناسه عددی تسک</param>
        /// <returns>
        /// نمونه‌ای از موجودیت تسک در صورت یافت شدن؛ در غیر این صورت null
        /// </returns>
        Task<TaskEntity> GetByIdAsync(int id);

        /// <summary>
        /// 🟣 دریافت تمام تسک‌ها (فقط داده‌های فعال)
        /// </summary>
        /// <returns>
        /// لیستی از موجودیت‌های تسک (TaskEntity)
        /// </returns>
        Task<List<TaskEntity>> GetAllAsync();

        /// <summary>
        /// 🔴 حذف سخت (اختیاری - غیر فعال پیش‌فرض)
        /// این متد صرفاً در شرایط خاص مورد استفاده قرار می‌گیرد.
        /// در ریرا، حذف پیش‌فرض به‌صورت SoftDelete انجام می‌شود.
        /// </summary>
        /// <param name="id">شناسه رکورد</param>
        /// <returns>مقدار true در صورت حذف موفق</returns>
        Task<bool> DeleteAsync(int id);
    }
}
