// ===========================================================
// 📘 RiRaDocs Teaching Edition (Farsi Inline)
// File: GlobalUsings.cs
// Layer: Tests → Common / Infrastructure Imports
// Context: Global using directives for Rira solution
// هدف: متمرکز کردن فضای‌نام‌های عمومی مورد استفاده در تست‌ها و سرویس‌ها
// انتشار: RiraDocs-v2025.11.4-Stable-Final-Fixed
// ===========================================================

// ✅ پوشش کتابخانه‌های اصلی تست واحد و ادغام (Unit & Integration)
global using Xunit;                // فریم‌ورک تست واحد، پایهٔ تمامی تست‌ها در پروژه
global using FluentAssertions;     // کتابخانه‌ی Assert خوانا و توصیفی برای بررسی نتایج تست‌ها

// ✅ پوشش فضاهای پروژه‌ی Rira برای استفاده در تست‌ها
global using Rira.Application;     // دسترسی به لایه Application (Services, Commands, Validators)
global using Rira.Persistence;     // دسترسی به لایه Persistence (DbContext, Configurations)
global using Rira.Application.Services; // سرویس‌های کاربردی (مانند TaskService, EmployeeService)
global using Rira.Application.DTOs;     // DTOهای انتقال داده برای ورودی و خروجی سرویس‌ها

// ✅ پوشش EF Core برای تست‌های مبتنی بر InMemory و Repository
global using Microsoft.EntityFrameworkCore; // ارجاع‌های EF برای DbContext، IQueryable و عملیات داده

// ===========================================================
// 📘 جمع‌بندی آموزشی (RiRaDocs Summary)
// -----------------------------------------------------------
// ▫ فایل GlobalUsings.cs تمامی فضای‌نام‌های مشترک را در سطح پروژه یک‌بار تعریف می‌کند.
// ▫ این رویکرد موجب حذف تکرار using‌ها از هر فایل تست و سرویس شده و خوانایی کد را افزایش می‌دهد.
// ▫ در معماری Clean، این فایل میان‌لایه‌ای است و به کل پروژه تست‌ها و Application دسترسی یکپارچه می‌دهد.
// ▫ تمامی تست‌های Application و Integration از این فایل استفاده می‌کنند.
// ▫ تگ انتشار نهایی: RiraDocs-v2025.11.4-Stable-Final-Fixed
// ===========================================================
