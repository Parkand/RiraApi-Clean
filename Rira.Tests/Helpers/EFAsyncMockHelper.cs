using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Rira.Tests.Common.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RiraApi.Tests.Helpers
{
    // 🧪 کلاس کمکی RiRaDocs برای تست واحد EF Core — EFAsyncMockHelper
    // ===============================================================================
    // این کلاس برای ایجاد Mock از DbSet<TEntity> در تست‌های واحد طراحی شده است؛
    // به‌طور کامل سازگار با EF Core 8 و تمام متدهای غیرهم‌زمان (Async) شامل:
    //   ▫ Add / AddAsync
    //   ▫ Remove
    //   ▫ FindAsync
    //   ▫ کوئری‌های IQueryable و IAsyncEnumerable
    //
    // 🎯 هدف آموزشی RiRaDocs:
    //     - جداسازی وابستگی به پایگاه داده در تست‌ها.
    //     - شبیه‌سازی رفتار واقعی EF Core برای CRUD تستی.
    //     - پشتیبانی از auto‑increment شناسه در درج‌های تستی.
    //
    // 👨‍💻 کاربرد:
    //     در فایل‌های تست UnitTests لایه Persistence یا Services، جهت ایجاد نسخه‌ی مجازی
    //     از DbSet مانند:
    //         var mockSet = EFAsyncMockHelper.BuildMockDbSet(tasksList);
    // ===============================================================================
    public static class EFAsyncMockHelper
    {
        // ---------------------------------------------------------------------------
        // 🧩 متد اصلی ساخت DbSet مجازی با پشتیبانی کامل از Async
        // ---------------------------------------------------------------------------
        // TEntity: نوع موجودیت EF (مثلاً TaskEntity یا EmployeeEntity)
        // source: مجموعه داده تستی اولیه که به عنوان حافظه‌ی مجازی داده‌ها عمل می‌کند.
        //
        // خروجی: Mock<DbSet<TEntity>> با رفتار واقعی EF شامل:
        //     Add(), AddAsync(), Remove(), FindAsync() و قابلیت LINQ Async پیمایشگر.
        // ---------------------------------------------------------------------------
        public static Mock<DbSet<TEntity>> BuildMockDbSet<TEntity>(List<TEntity> source)
            where TEntity : class
        {
            var queryableData = source.AsQueryable();
            var mockSet = new Mock<DbSet<TEntity>>();

            // ✅ پیاده‌سازی کامل IQueryable برای پشتیبانی از LINQ در Mock
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Provider)
                   .Returns(new TestAsyncQueryProvider(queryableData.Provider));
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Expression)
                   .Returns(queryableData.Expression);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType)
                   .Returns(queryableData.ElementType);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator())
                   .Returns(queryableData.GetEnumerator());

            // ✅ پشتیبانی از IAsyncEnumerable<TEntity>
            // این بخش امکان اجرای ToListAsync(), FirstOrDefaultAsync() و سایر متدهای
            // غیرهم‌زمان LINQ را فراهم می‌کند.
            mockSet.As<IAsyncEnumerable<TEntity>>()
                   .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                   .Returns(new TestAsyncEnumerator<TEntity>(queryableData.GetEnumerator()));

            // -----------------------------------------------------------------------
            // 🟢 متد Add (هم‌زمان)
            // -----------------------------------------------------------------------
            // در تست، موجودیت جدید را به لیست داده‌ها اضافه می‌کند و فیلد Id را
            // در صورت وجود و مقدار صفر به صورت خودکار مقداردهی می‌کند.
            mockSet.Setup(d => d.Add(It.IsAny<TEntity>()))
                   .Callback<TEntity>(entity =>
                   {
                       AssignAutoIdIfExists(entity, source);
                       source.Add(entity);
                   })
                   .Returns<TEntity>(entity =>
                   {
                       // برگرداندن شئ EntityEntry<TEntity> مجازی برای حفظ سازگاری EF
                       var entryMock = new Mock<EntityEntry<TEntity>>();
                       entryMock.SetupGet(e => e.Entity).Returns(entity);
                       return entryMock.Object;
                   });

            // -----------------------------------------------------------------------
            // 🟣 متد AddAsync (غیرهم‌زمان – سازگار با EF Core 8)
            // -----------------------------------------------------------------------
            // معادل Async و بازگرداننده ValueTask<EntityEntry<TEntity>>.
            // از EF8 الگوی برگشتی ValueTask به‌جای Task برای کارایی بهتر معرفی شده.
            mockSet.Setup(d => d.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
                   .Returns((TEntity entity, CancellationToken token) =>
                   {
                       AssignAutoIdIfExists(entity, source);
                       source.Add(entity);

                       var entryMock = new Mock<EntityEntry<TEntity>>();
                       entryMock.SetupGet(e => e.Entity).Returns(entity);
                       return new ValueTask<EntityEntry<TEntity>>(entryMock.Object);
                   });

            // -----------------------------------------------------------------------
            // 🔴 متد Remove (حذف موجودیت از حافظه تستی)
            // -----------------------------------------------------------------------
            mockSet.Setup(d => d.Remove(It.IsAny<TEntity>()))
                   .Callback<TEntity>(entity =>
                   {
                       if (source.Contains(entity))
                           source.Remove(entity);
                   });

            // -----------------------------------------------------------------------
            // 🔍 متد FindAsync (جستجوی موجودیت بر اساس Id)
            // -----------------------------------------------------------------------
            // شبیه‌سازی کامل رفتار EF.Core برای FindAsync(params object[] keys)
            // بررسی می‌کند آیا پراپرتی "Id" وجود دارد یا نه و مقدار آن را تطبیق می‌دهد.
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                   .Returns<object[]>(ids =>
                   {
                       if (ids == null || ids.Length == 0)
                           return ValueTask.FromResult<TEntity?>(null);

                       var idProp = typeof(TEntity).GetProperty("Id");
                       if (idProp == null)
                           return ValueTask.FromResult<TEntity?>(null);

                       int id = (int)ids[0];
                       var match = source.FirstOrDefault(e =>
                       {
                           var val = idProp.GetValue(e);
                           return val is int i && i == id;
                       });

                       // اگر یافت نشود، نمونه جدیدی از TEntity برمی‌گرداند (رفتار تستی)
                       return ValueTask.FromResult(match ?? Activator.CreateInstance<TEntity>());
                   });

            return mockSet;
        }

        // ---------------------------------------------------------------------------
        // 🔢 متد خصوصی اختصاص خودکار ID به موجودیت‌های جدید
        // ---------------------------------------------------------------------------
        // این تابع مقدار فعلی بزرگ‌ترین شناسه را یافته و شناسه جدید را برابر maxId+1
        // تنظیم می‌کند. در تست‌های Insert از Mock، معادل Identity آتو‌اینکریمنت دیتابیس است.
        private static void AssignAutoIdIfExists<TEntity>(TEntity entity, List<TEntity> source)
            where TEntity : class
        {
            var idProp = typeof(TEntity).GetProperty("Id");
            if (idProp == null || idProp.PropertyType != typeof(int))
                return;

            var currentId = (int?)idProp.GetValue(entity) ?? 0;
            if (currentId > 0) return;

            var maxId = source
                .Select(e => (int?)idProp.GetValue(e))
                .Where(v => v.HasValue)
                .DefaultIfEmpty(0)
                .Max() ?? 0;

            idProp.SetValue(entity, maxId + 1);
        }
    }

    // ===============================================================================
    // 📘 خلاصه RiRaDocs Teaching Edition
    // ------------------------------------------------------------------------------
    // 🔹 نقش EFAsyncMockHelper در معماری تست پروژه:
    //     ▫ بخشی از لایه تست کمکی (Test Helpers) برای شبیه‌سازی پایگاه داده Memory.
    //     ▫ مکمل کلاس‌های TestAsyncQueryProvider و TestAsyncEnumerator.
    //
    // 🔹 نکات فنی:
    //     ▫ EF Core 8 از ValueTask در AddAsync استفاده می‌کند و این نسخه کاملاً با آن هماهنگ است.
    //     ▫ در Mock‌های Moq، EntityEntry باید از طریق Mock<EntityEntry<TEntity>> ساخته شود.
    //     ▫ FindAsync از Reflection برای تشخیص پراپرتی Id استفاده می‌کند تا با موجودیت‌های مختلف کار کند.
    //
    // 🔹 مزیت آموزشی:
    //     ▫ مناسب‌ترین روش ساخت DbSet تستی بدون نیاز به In‑Memory Database.
    //     ▫ آموزش کاربردی مفاهیم IQueryable و IAsyncEnumerable در سطح EF Core.
    //
    // 🔹 تگ انتشار RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===============================================================================
}
