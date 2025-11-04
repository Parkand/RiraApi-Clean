// ===========================================================
// 📘 RiRaDocs Teaching Edition (Farsi Inline)
// File: InMemoryContextFactory.cs
// Layer: Tests → TestUtilities
// Context: EF Core 8 – InMemory Provider Utility
// هدف: ساخت DbContext مستقل برای تست‌ها بدون اتصال به پایگاه واقعی
// انتشار: RiraDocs-v2025.11.4-Stable-Final-Fixed
// ===========================================================

using Microsoft.EntityFrameworkCore;
using Rira.Persistence.Data;

namespace Rira.Tests.TestUtilities
{
    /// <summary>
    /// 🧠 کلاس کمکی ایجاد DbContext حافظه‌ای برای تست‌ها (InMemory Context)
    /// ---------------------------------------------------------------
    /// 🎯 هدف آموزشی RiRaDocs:
    ///   ▫ آموزش مفهوم Database Provider حافظه‌ای EF Core (InMemory)
    ///   ▫ فراهم‌سازی محیط تست تمیز، ایزوله و قابل تکرار در هر اجرا
    ///   ▫ حذف وابستگی به دیتابیس واقعی جهت تسریع تست‌ها
    /// ---------------------------------------------------------------
    /// کاربرد:
    ///   این کلاس در تست‌های واحد و ادغام (Integration/Unit) استفاده می‌شود
    ///   تا هر تست بتواند یک کانتکست جدید و مستقل بسازد.
    /// </summary>
    public static class InMemoryContextFactory
    {
        // ===============================================================
        // 🧩 متد اصلی ساخت DbContext در حافظه (بدون دیتابیس واقعی)
        // ---------------------------------------------------------------
        // 🎯 توضیحات آموزشی:
        //   ▫ EFCore دارای Provider مخصوصی به نام "InMemoryDatabase" است
        //     که تمام عملیات CRUD را در حافظه (RAM) شبیه‌سازی می‌کند.
        //   ▫ استفاده از Guid.NewGuid() برای داشتن نام پایگاه یکتا باعث می‌شود
        //     هر تست محیط کاملاً مستقل داشته باشد و داده‌های تست دیگران نشت نکنند.
        //   ▫ این متد در تست‌های مربوط به Repository، Service و Validator فراخوانی می‌شود.
        // ===============================================================
        public static AppDbContext CreateDbContext()
        {
            // 👇 ساخت گزینه‌های پیکربندی با Provider حافظه‌ای (InMemory)
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // نام منحصربه‌فرد برای هر تست
                .Options;

            // ساخت نمونه جدید از AppDbContext با گزینه‌های تعریف‌شده
            return new AppDbContext(options);
        }
    }
}

// ===========================================================
// 📘 جمع‌بندی آموزشی (RiRaDocs Summary)
// -----------------------------------------------------------
// ▫ InMemoryContextFactory ابزاری ساده ولی کلیدی در تست‌های پروژه است.
// ▫ هر بار فراخوانی متد CreateDbContext، دیتابیس کاملاً جدیدی می‌سازد.
// ▫ این روش از آلودگی داده‌ها میان تست‌ها (Test Pollution) جلوگیری می‌کند.
// ▫ Provider InMemory هیچ اتصال واقعی به SQL ندارد و فقط در حافظه عمل می‌کند.
// ▫ در تست‌های Repository‌ها و سرویس‌ها از آن برای ساخت کانتکست مؤقت استفاده می‌شود.
// ▫ تگ انتشار: RiraDocs-v2025.11.4-Stable-Final-Fixed
// ===========================================================
