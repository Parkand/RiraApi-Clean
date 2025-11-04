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
    /// 🧠 کلاس کمکی برای ساخت DbSet‌های موک‌شده سازگار با EF Core 8.
    /// شامل متدهای ساخت و رفتار CRUD جهت استفاده در تست‌های واحد پروژه ریرا.
    /// </summary>
    public static class EFAsyncMockHelper
    {
        // ------------------------------------------------------------------------------
        // 📦 متد اصلی ساخت Mock از DbSet<TEntity>
        // ------------------------------------------------------------------------------
        public static Mock<DbSet<TEntity>> CreateMockDbSet<TEntity>(List<TEntity> data)
            where TEntity : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<TEntity>>();

            // 📌 تنظیم ویژگی‌های IQueryable برای پشتیبانی از LINQ و LINQ Async
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider(queryable.Provider));
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Expression)
                .Returns(queryable.Expression);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType)
                .Returns(queryable.ElementType);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator())
                .Returns(queryable.GetEnumerator());
            mockSet.As<IAsyncEnumerable<TEntity>>()
                .Setup(d => d.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<TEntity>(data.GetEnumerator()));

            // ─────────────────── 📧 رفتارهای CRUD
            // ✅ Add
            mockSet.Setup(m => m.Add(It.IsAny<TEntity>()))
                   .Callback<TEntity>(e => AssignAutoIdIfExists(e, data))
                   .Returns((TEntity e) => { data.Add(e); return e; });

            // ✅ Remove
            mockSet.Setup(m => m.Remove(It.IsAny<TEntity>()))
                   .Callback<TEntity>(entity => data.Remove(entity));

            // ✅ AddAsync
            mockSet.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
                   .Returns<TEntity, CancellationToken>((entity, token) =>
                   {
                       AssignAutoIdIfExists(entity, data);
                       data.Add(entity);
                       var entry = Mock.Of<EntityEntry<TEntity>>(e => e.Entity == entity);
                       return ValueTask.FromResult(entry);
                   });

            // ✅ FindAsync (برای جلوگیری از NullReference هنگام Update)
            mockSet.Setup(m => m.FindAsync(It.IsAny<object[]>()))
                   .Returns<object[]>(ids =>
                   {
                       var id = (int)ids[0];
                       var found = data.SingleOrDefault(e =>
                           (int)e.GetType().GetProperty("Id")!.GetValue(e)! == id);

                       // اگر پیدا نشد، نمونه خالی بساز تا سرویس نپره
                       return ValueTask.FromResult(found ?? Activator.CreateInstance<TEntity>());
                   });

            return mockSet;
        }

        // ------------------------------------------------------------------------------
        // 📎 متد سازگار با تست‌های قدیمی (BuildMockDbSet)
        // ------------------------------------------------------------------------------
        public static Mock<DbSet<TEntity>> BuildMockDbSet<TEntity>(List<TEntity> data)
            where TEntity : class =>
            // 👇 اضافه شده برای رفع خطای CS0121
            Rira.Tests.Common.Mocks.EFAsyncMockHelper.CreateMockDbSet<TEntity>(data);

        // ------------------------------------------------------------------------------
        // 🔢 تخصیص ID خودکار به رکوردهای جدید در Mock
        // ------------------------------------------------------------------------------
        private static void AssignAutoIdIfExists<TEntity>(TEntity entity, List<TEntity> data)
        {
            // یافتن پراپرتی Id
            var idProp = typeof(TEntity).GetProperty("Id");
            if (idProp == null || idProp.PropertyType != typeof(int))
                return;

            // مقدار فعلی Id را بخوان
            var currentIdObj = idProp.GetValue(entity);
            var currentId = currentIdObj is int i ? i : 0;

            // فقط اگر Id فعلی صفر است ...
            if (currentId == 0)
            {
                // پیدا کردن بزرگ‌ترین Id فعلی در داده‌ها
                var maxId = data
                    .Select(e => (int?)idProp.GetValue(e))
                    .Where(v => v.HasValue)
                    .DefaultIfEmpty(0)
                    .Max() ?? 0;

                // مقدار جدید (maxId + 1)
                var newId = maxId + 1;
                idProp.SetValue(entity, newId);
            }
        }
    }
}
