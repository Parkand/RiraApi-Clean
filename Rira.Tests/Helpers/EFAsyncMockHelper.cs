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
    /// <summary>
    /// نسخه نهایی EFAsyncMockHelper — سازگار با EF Core 8 و بدون خطاهای کامپایل.
    /// هندل کننده‌ی کامل متدهای Add، AddAsync، Remove، FindAsync و کوئری‌های Async.
    /// </summary>
    public static class EFAsyncMockHelper
    {
        public static Mock<DbSet<TEntity>> BuildMockDbSet<TEntity>(List<TEntity> source)
            where TEntity : class
        {
            var queryableData = source.AsQueryable();
            var mockSet = new Mock<DbSet<TEntity>>();

            // ✅ پشتیبانی از IQueryable
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Provider)
                   .Returns(new TestAsyncQueryProvider(queryableData.Provider));
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Expression)
                   .Returns(queryableData.Expression);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType)
                   .Returns(queryableData.ElementType);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator())
                   .Returns(queryableData.GetEnumerator());

            // ✅ پشتیبانی از IAsyncEnumerable<TEntity>
            mockSet.As<IAsyncEnumerable<TEntity>>()
                   .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                   .Returns(new TestAsyncEnumerator<TEntity>(queryableData.GetEnumerator()));

            //───────────────────────────────
            // 🟢 Add (هم‌زمان)
            //───────────────────────────────
            mockSet.Setup(d => d.Add(It.IsAny<TEntity>()))
                   .Callback<TEntity>(entity =>
                   {
                       AssignAutoIdIfExists(entity, source);
                       source.Add(entity);
                   })
                   // برگرداندن EntityEntry<TEntity> معتبر
                   .Returns<TEntity>(entity =>
                   {
                       var entryMock = new Mock<EntityEntry<TEntity>>();
                       entryMock.SetupGet(e => e.Entity).Returns(entity);
                       return entryMock.Object;
                   });

            //───────────────────────────────
            // 🟣 AddAsync (غیرهم‌زمان و کاملاً سازگار با EF8)
            //───────────────────────────────
            mockSet.Setup(d => d.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
                   .Returns((TEntity entity, CancellationToken token) =>
                   {
                       AssignAutoIdIfExists(entity, source);
                       source.Add(entity);

                       var entryMock = new Mock<EntityEntry<TEntity>>();
                       entryMock.SetupGet(e => e.Entity).Returns(entity);

                       // نوع برگشتی درست EF8
                       return new ValueTask<EntityEntry<TEntity>>(entryMock.Object);
                   });

            //───────────────────────────────
            // 🔴 Remove
            //───────────────────────────────
            mockSet.Setup(d => d.Remove(It.IsAny<TEntity>()))
                   .Callback<TEntity>(entity =>
                   {
                       if (source.Contains(entity))
                           source.Remove(entity);
                   });

            //───────────────────────────────
            // 🔍 FindAsync
            //───────────────────────────────
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

                       return ValueTask.FromResult(match ?? Activator.CreateInstance<TEntity>());
                   });

            return mockSet;
        }

        //───────────────────────────────
        // 🔢 تخصیص خودکار ID
        //───────────────────────────────
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
}
