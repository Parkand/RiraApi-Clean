using MediatR;
using Rira.Application.Common;
using static Rira.Application.DTOs.EmployeeDTO;

namespace Rira.Application.Features.Employees.Commands.UpdateEmployee
{
    /// <summary>
    /// 🎯 فرمان ویرایش اطلاعات کامل کارمند موجود در سیستم.
    /// بر اساس مدل دامنه EmployeeEntity و هماهنگ با Mapper و Validator ریرا.
    /// </summary>
    public class EmployeeUpdateCommand : IRequest<ResponseModel<int>>
    {
        /// <summary>
        /// شناسه‌ی کارمند هدف برای ویرایش
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// نام کوچک کارمند
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// نام خانوادگی کارمند
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// شماره موبایل (باید یکتا باشد)
        /// </summary>
        public string? MobileNumber { get; set; }

        /// <summary>
        /// ایمیل کارمند
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// تاریخ تولد شمسی (فرمت yyyy/MM/dd)
        /// </summary>
        public string? BirthDatePersian { get; set; }

        /// <summary>
        /// جنسیت (True=مرد، False=زن)
        /// </summary>
        public bool? IsMale { get; set; }

        /// <summary>
        /// سطح تحصیلات از نوع Enum (Diploma=1, Bachelor=3,...)
        /// </summary>
        public EducationLevelType? EducationLevel { get; set; }

        /// <summary>
        /// کد ملی (اختیاری)
        /// </summary>
        public string? NationalCode { get; set; }

        /// <summary>
        /// عنوان شغلی کارمند
        /// </summary>
        public string? JobTitle { get; set; }

        /// <summary>
        /// تاریخ استخدام (به UTC ذخیره می‌شود)
        /// </summary>
        public DateTime? HireDateUtc { get; set; }

        /// <summary>
        /// وضعیت فعال بودن کارمند
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// توضیحات اضافی
        /// </summary>
        public string? Description { get; set; }
    }
}
