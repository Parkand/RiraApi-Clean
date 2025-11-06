// ===========================================================
// 📘 RiRaDocs Teaching Edition – Interface Layer
// ===========================================================
// File: IEmployeeService.cs
// Author: RiRaDocs v2025.11.5 – Stable Guid Migration Edition
// Description:
// رابط استاندارد سرویس کارمندان با پیروی از ساختار Clean Architecture.
// تمامی متدها از نوع Async بوده و پاسخ را در قالب مدل استاندارد ResponseModel<T> بازمی‌گردانند.
// ===========================================================

using Rira.Application.Common;
using Rira.Application.DTOs;

namespace Rira.Application.Interfaces
{
    /// <summary>
    /// 🔹 رابط سرویس کارمندان (Employee Service Interface)
    /// وظیفه دارد کلیه عملیات CRUD را بر روی داده‌های کارمند مدیریت کند.
    /// از Guid به عنوان شناسه اصلی استفاده می‌کند.
    /// </summary>
    public interface IEmployeeService
    {
        // ===========================================================
        // 🟢 READ OPERATIONS
        // ===========================================================

        /// <summary>
        /// دریافت لیست کامل کارمندان.
        /// </summary>
        /// <returns>لیستی از DTOهای کارمندان در قالب ResponseModel.</returns>
        Task<ResponseModel<List<EmployeeDTO>>> GetAllAsync();

        /// <summary>
        /// دریافت اطلاعات یک کارمند با شناسه یکتا (Guid).
        /// </summary>
        /// <param name="id">شناسه کارمند.</param>
        /// <returns>شیء EmployeeDTO در قالب ResponseModel.</returns>
        Task<ResponseModel<EmployeeDTO>> GetByIdAsync(Guid id);

        // ===========================================================
        // 🟡 CREATE / UPDATE
        // ===========================================================

        /// <summary>
        /// ایجاد رکورد جدید کارمند.
        /// </summary>
        /// <param name="dto">مدل داده‌ی ورودی.</param>
        /// <returns>DTO ایجادشده در قالب ResponseModel.</returns>
        Task<ResponseModel<EmployeeDTO>> CreateAsync(EmployeeDTO dto);

        /// <summary>
        /// بروزرسانی اطلاعات کارمند موجود.
        /// </summary>
        /// <param name="dto">مدل داده‌ی بروزشده.</param>
        /// <returns>DTO بروزشده در قالب ResponseModel.</returns>
        Task<ResponseModel<EmployeeDTO>> UpdateAsync(EmployeeDTO dto);

        // ===========================================================
        // 🔴 DELETE OPERATION
        // ===========================================================

        /// <summary>
        /// حذف کارمند بر اساس شناسه یکتا.
        /// </summary>
        /// <param name="id">شناسه کارمند مورد حذف.</param>
        /// <returns>نتیجه بولی عملیات در قالب ResponseModel.</returns>
        Task<ResponseModel<bool>> DeleteAsync(Guid id);
    }
}
