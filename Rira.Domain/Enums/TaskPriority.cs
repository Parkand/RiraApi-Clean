namespace Rira.Domain.Enums
{
    /// <summary>
    /// سطوح اولویت تسک‌ها در سامانه ریرا (جهت استفاده در Command، DTO و Entity)
    /// </summary>
    public enum TaskPriority
    {
        /// <summary>
        /// پایین‌ترین سطح اولویت
        /// </summary>
        Low = 1,

        /// <summary>
        /// اولویت متوسط
        /// </summary>
        Medium = 2,

        /// <summary>
        /// اولویت بالا
        /// </summary>
        High = 3,

        /// <summary>
        /// اولویت بحرانی - نیازمند اقدام سریع
        /// </summary>
        Critical = 4
    }
}
