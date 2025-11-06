// ===========================================================
// 📘 RiRaDocs Teaching Edition
// File: EmployeeUpdateCommand.cs
// Layer: Application – Command (CQRS)
// نسخه: RiraDocs‑v2025.11.5‑Stable‑Final‑Fixed
// ===========================================================

using MediatR;
using Rira.Application.Common;
using System;

namespace Rira.Application.Features.Employees.Commands.UpdateEmployee
{
    /// <summary>
    /// 🧩 فرمان بروزرسانی اطلاعات کارمند — بخش Command در الگوی CQRS
    /// </summary>
    public class EmployeeUpdateCommand : IRequest<ResponseModel<int>>   // ✅ افزوده شد
    {
        public Guid Id { get; set; }              // 🔄 اصلاح از int به Guid

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string Description { get; set; } = string.Empty;
    }
}
