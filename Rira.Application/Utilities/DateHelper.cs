using System.Globalization;

namespace Rira.Application.Utilities
{
    /// <summary>
    /// ⏰ کلاس کمکی برای کار با تاریخ شمسی (Persian Calendar)
    /// در پروژه ریرا تمام فیلدهای تاریخی از نوع string و با فرمت "yyyy/MM/dd" هستند.
    /// این کلاس توابعی برای تبدیل تاریخ‌ها و تولید زمان فعلی شمسی ارائه می‌دهد.
    /// </summary>
    public static class DateHelper
    {
        /// <summary>
        /// 🔁 تبدیل یک تاریخ میلادی به رشته‌ای از نوع تاریخ شمسی.
        /// فرمت خروجی همیشه "yyyy/MM/dd" است — مثال: "1404/07/19".
        /// </summary>
        /// <param name="date">تاریخ میلادی (DateTime)</param>
        /// <returns>رشته‌ی تاریخ شمسی با فرمت استاندارد</returns>
        public static string ToPersianDateString(DateTime date)
        {
            var pc = new PersianCalendar();
            return $"{pc.GetYear(date):0000}/{pc.GetMonth(date):00}/{pc.GetDayOfMonth(date):00}";
        }

        /// <summary>
        /// 🕒 دریافت تاریخ فعلی سیستم به‌صورت شمسی.
        /// از این متد برای مقداردهی فیلدهای CreatedAt و UpdatedAt در زمان ساخت یا بروزرسانی تسک استفاده می‌شود.
        /// </summary>
        /// <returns>تاریخ فعلی شمسی با فرمت "yyyy/MM/dd"</returns>
        public static string GetPersianNow()
        {
            return ToPersianDateString(DateTime.Now);
        }
    }
}
