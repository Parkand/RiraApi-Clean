// ===========================================================
// 📘 RiRaDocs Teaching Edition (Farsi Inline)
// File: EFAsyncMockHelper.cs
// Layer: Tests → Common → Mocks
// Context: Mocking EF Core 8 Asynchronous DbSet for Unit Testing
// هدف: ساخت DbSetهای موک‌شده با پشتیبانی از LINQ Async جهت اجرای تست‌ها بدون دیتابیس واقعی.
// انتشار: RiraDocs-v2025.11.4-Stable-Final-Fixed
// ===========================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rira.Tests.Common.Mocks
{
    /// <summary>
    /// 🧠 کلاس کمکی ساخت DbSet Mock برای EF Core 8
    /// -------------------------------------------------------------
    /// 🎯 هدف آموزشی RiRaDocs:
    ///   ▫ آموزش نحوه‌ی شبیه‌سازی رفتار واقعی EF Core بدون دیتابیس
    ///   ▫ پشتیبانی از متدهای IQueryable و IAsyncEnumerable (LINQ Async)
    ///   ▫ به‌کارگیری List<T> به‌جای جدول برای تست CRUD
    ///   ▫ شبیه‌سازی صحیح متدهای AddAsync و FindAsync EF
    /// -------------------------------------------------------------
    /// استفاده: در تمامی Unit Testهای لایه‌ی Application و Repository
    /// که نیاز به محیط EFCore در حافظه دارند.
    /// </summary>
    public static class EFAsyncMockHelper
    {
        // ===============================================================
        // 📦 متد اصلی: ایجاد Mock از DbSet<TEntity>
        // ---------------------------------------------------------------
        // 🎯 هدف آموزشی:
        //     ▫ آشنایی با نحوه‌ی تزریق رفتار IQueryable در Mock
        //     ▫ شبیه‌سازی کامل رفتار EF در LINQ و LINQ Async
        //     ▫ افزودن رفتار CRUD پایه برای محیط تست
        // ===============================================================
        public static Mock<DbSet<TEntity>> CreateMockDbSet<TEntity>(List<TEntity> data)
            where TEntity : class
        {
            // لیست داده‌ها به IQueryable تبدیل می‌شود تا دستورات Where, Select, ... کار کنند.
            var queryable = data.AsQueryable();

            // ساخت نمونه Mock از DbSet
            var mockSet = new Mock<DbSet<TEntity>>();

            // ---------------------------------------------------------------
            // ⚙️ پیکربندی Interfaceهای IQueryable و IAsyncEnumerable
            // ---------------------------------------------------------------
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider(queryable.Provider)); // استفاده از Provider سفارشی Async
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Expression)
                .Returns(queryable.Expression);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType)
                .Returns(queryable.ElementType);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator())
                .Returns(queryable.GetEnumerator());

            // ✅ تنظیم پشتیبانی از await foreach (LINQ Async)
            mockSet.As<IAsyncEnumerable<TEntity>>()
                .Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<TEntity>(data.GetEnumerator()));

            // ---------------------------------------------------------------
            // 🧩 تنظیم رفتار CRUD در Mock
            // ---------------------------------------------------------------

            // ✅ Add → اضافه کردن موجودیت جدید به لیست داده‌ها
            mockSet.Setup(m => m.Add(It.IsAny<TEntity>()))
                   .Callback<TEntity>(e => AssignAutoIdIfExists(e, data))   // تخصیص Id خودکار
                   .Returns((TEntity e) =>
                   {
                       data.Add(e);
                       return e;
                   });

            // ✅ Remove → حذف موجودیت از حافظه
            mockSet.Setup(m => m.Remove(It.IsAny<TEntity>()))
                   .Callback<TEntity>(entity => data.Remove(entity));

            // ✅ AddAsync → شبیه‌سازی عملکرد EF هنگام افزودن غیرهم‌زمان
            // (برمی‌گرداند EntityEntry<TEntity> مشابه EF واقعی)
            mockSet.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
                   .Returns<TEntity, CancellationToken>((entity, token) =>
                   {
                       AssignAutoIdIfExists(entity, data);
                       data.Add(entity);
                       var entry = Mock.Of<EntityEntry<TEntity>>(e => e.Entity == entity);
                       return ValueTask.FromResult(entry);
                   });

            // ✅ FindAsync → یافتن موجودیت بر اساس کلید اولیه (Id)
            // این پیاده‌سازی مانع NullReferenceException در تست Update می‌شود.
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                   .Returns<object[]>(ids =>
                   {
                       var id = (int)ids[0];
                       var found = data.SingleOrDefault(e =>
                           (int)e.GetType().GetProperty("Id")!.GetValue(e)! == id);

                       // در صورت نبود رکورد، نمونه جدید ایجاد می‌شود تا منطق سرویس دچار Null نشود.
                       return ValueTask.FromResult(found ?? Activator.CreateInstance<TEntity>());
                   });

            // نهایی‌سازی Mock ساخت‌شده
            return mockSet;
        }

        // ===============================================================
        // 🧩 متد سازگار با نسخه‌های قدیمی‌تر تست‌ها (Backwards Compatibility)
        // ---------------------------------------------------------------
        // 🎯 هدف: اطمینان از اینکه تست‌های قدیمی که از BuildMockDbSet استفاده کرده‌اند،
        //        همچنان بدون تغییر کار می‌کنند.
        // ===============================================================
        public static Mock<DbSet<TEntity>> BuildMockDbSet<TEntity>(List<TEntity> data)
            where TEntity : class =>
            // فراخوانی مستقیم متد جدید برای جلوگیری از کد تکراری
            Rira.Tests.Common.Mocks.EFAsyncMockHelper.CreateMockDbSet<TEntity>(data);

        // ===============================================================
        // 🔢 تخصیص خودکار Id به رکوردهای جدید
        // ---------------------------------------------------------------
        // 🎯 هدف آموزشی:
        //     ▫ شبیه‌سازی رفتار Identity در EFCore
        //     ▫ پیشگیری از درج Id=0 در داده‌ها
        // ===============================================================
        private static void AssignAutoIdIfExists<TEntity>(TEntity entity, List<TEntity> data)
        {
            // بررسی وجود پراپرتی Id در نوع موجودیت
            var idProp = typeof(TEntity).GetProperty("Id");
            if (idProp == null || idProp.PropertyType != typeof(int))
                return;

            // مقدار فعلی Id را بخوانیم
            var currentIdObj = idProp.GetValue(entity);
            var currentId = currentIdObj is int i ? i : 0;

            // فقط اگر Id صفر است (یعنی هنوز تعیین نشده)
            if (currentId == 0)
            {
                // یافتن بیشترین مقدار Id موجود در داده‌ها
                var maxId = data
                    .Select(e => (int?)idProp.GetValue(e))
                    .Where(v => v.HasValue)
                    .DefaultIfEmpty(0)
                    .Max() ?? 0;

                // تخصیص Id جدید (max + 1)
                var newId = maxId + 1;
                idProp.SetValue(entity, newId);
            }
        }
    }
}

// ===========================================================
// 📘 جمع‌بندی آموزشی (RiRaDocs Summary)
// -----------------------------------------------------------
// ▫ EFAsyncMockHelper ابزاری برای شبیه‌سازی رفتار EFCore 8 است
//   که نیاز به DB واقعی را در تست‌ها حذف می‌کند.
// ▫ با بهره‌گیری از TestAsyncQueryProvider و TestAsyncEnumerator:
//     → متدهای LINQ Async مانند ToListAsync() و FirstOrDefaultAsync()
//       در تست‌ها بدون نیاز به اتصال دیتابیس کار می‌کنند.
// ▫ CRUD در حافظه انجام می‌شود و AutoId دقیقاً مانند Identity Sql عمل می‌کند.
// ▫ سازگاری کامل با تست‌های قدیمی (BuildMockDbSet) حفظ شده است.
// ▫ این کلاس در تمام تست‌های Unit لایه Application و Persistence مورد استفاده است.
// ▫ تگ انتشار: RiraDocs-v2025.11.4-Stable-Final-Fixed
// ===========================================================
