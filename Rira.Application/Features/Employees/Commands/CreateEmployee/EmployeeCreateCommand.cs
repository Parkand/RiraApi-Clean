// ===========================================================
// 📘 RiRaDocs Teaching Edition
// File: EmployeeCreateCommand.cs
// Layer: Application – Command (CQRS)
// نسخه: RiraDocs‑v2025.11.5‑FixMediatRBase
// ===========================================================

using MediatR;
using Rira.Application.Common;
using System;

namespace Rira.Application.Features.Employees.Commands.CreateEmployee
{
    /// <summary>
    /// 🧩 فرمان ایجاد کارمند جدید (Command در الگوی CQRS)
    /// </summary>
    public class EmployeeCreateCommand : IRequest<ResponseModel<int>>   // ✅ افزوده شد: IRequest<ResponseModel<int>>
    {
        public Guid Id { get; set; } = Guid.NewGuid();  // 🔄 اصلاح از int به Guid و مقداردهی اولیه

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public DateTime HireDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string Description { get; set; } = string.Empty;
    }
}
