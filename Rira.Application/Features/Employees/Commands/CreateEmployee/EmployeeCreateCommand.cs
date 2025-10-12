using MediatR;
using Rira.Application.Common;
using static Rira.Application.DTOs.EmployeeDTO;

namespace Rira.Application.Features.Employees.Commands.CreateEmployee
{
    /// <summary>
    /// 🧾 فرمان ایجاد کارمند جدید در سیستم.
    /// شامل اطلاعات پایه‌ای کارمند است که باید اعتبارسنجی شود.
    /// </summary>
    public class EmployeeCreateCommand : IRequest<ResponseModel<int>>
    {
        /// <summary>
        /// نام کوچک کارمند
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// نام خانوادگی کارمند
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// شماره موبایل (یکتا و معتبر)
        /// </summary>
        public string MobileNumber { get; set; } = string.Empty;

        /// <summary>
        /// ایمیل (اختیاری ولی باید فرمت معتبر باشد)
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// تاریخ تولد شمسی (فرمت yyyy/MM/dd)
        /// </summary>
        public string BirthDatePersian { get; set; } = string.Empty;

        /// <summary>
        /// سطح تحصیلات از نوع Enum
        /// </summary>
        public EducationLevelType? EducationLevel { get; set; }

        /// <summary>
        /// جنسیت (True=مرد، False=زن)
        /// </summary>
        public bool IsMale { get; set; }
    }
}
