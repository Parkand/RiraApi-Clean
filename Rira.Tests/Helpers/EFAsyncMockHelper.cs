using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rira.Tests.Helpers
{
    /// <summary>
    /// 🔩 RiraDocs v2025.10.2
    /// EFAsyncMockHelper - نسخه‌ی نهایی با شبیه‌سازی کامل EF.
    /// پشتیبانی از AddAsync و SaveChangesAsync با تخصیص خودکار شناسه‌ی افزایشی.
    /// </summary>
    public static class EFAsyncMockHelper
    {
        /// <summary>
        /// ساخت DbSet موک‌شده با رفتار Async مشابه EF Core.
        /// </summary>
        public static Mock<DbSet<TEntity>> BuildMockDbSet<TEntity>(List<TEntity> source)
            where TEntity : class
        {
            var queryable = source.AsQueryable();
            var mockSet = new Mock<DbSet<TEntity>>();

            // پشتیبانی از async query
            mockSet.As<IAsyncEnumerable<TEntity>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<TEntity>(queryable.GetEnumerator()));

            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<TEntity>(queryable.Provider));
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            // ✅ AddAsync با تخصیص فوری شناسه افزایشی
            mockSet.Setup(m => m.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
                .Returns<TEntity, CancellationToken>((entity, _) =>
                {
                    AssignIdIncrement(entity, source);
                    source.Add(entity);

                    var entryMock = new Mock<EntityEntry<TEntity>>();
                    entryMock.Setup(e => e.Entity).Returns(entity);
                    return new ValueTask<EntityEntry<TEntity>>(entryMock.Object);
                });

            // حذف
            mockSet.Setup(m => m.Remove(It.IsAny<TEntity>()))
                .Callback((TEntity entity) => source.Remove(entity));

            return mockSet;
        }

        /// <summary>
        /// ساخت Mock از DbContext با منطق SaveChangesAsync واقعی.
        /// </summary>
        public static Mock<TContext> BuildMockDbContext<TContext, TEntity>(List<TEntity> source)
            where TContext : DbContext
            where TEntity : class
        {
            var mockSet = BuildMockDbSet(source);
            var mockContext = new Mock<TContext>();

            // برای دسترسی به DbSet<TEntity>
            mockContext.Setup(c => c.Set<TEntity>()).Returns(mockSet.Object);

            // ✅ شبیه‌سازی SaveChangesAsync — تخصیص شناسه برای داده‌های بدون Id
            mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() =>
                {
                    foreach (var entity in source)
                    {
                        AssignIdIncrement(entity, source);
                    }
                })
                .ReturnsAsync(1);

            return mockContext;
        }

        /// <summary>
        /// متد مشترک برای اختصاص شناسه افزایشی.
        /// </summary>
        private static void AssignIdIncrement<TEntity>(TEntity entity, List<TEntity> source)
        {
            var idProp = typeof(TEntity).GetProperty("Id");
            if (idProp == null || idProp.PropertyType != typeof(int))
                return;

            var currentValue = (int)idProp.GetValue(entity)!;
            if (currentValue == 0)
            {
                var nextId = source.Count > 0 ? source.Max(e => (int)idProp.GetValue(e)!) + 1 : 1;
                idProp.SetValue(entity, nextId);
            }
        }
    }

    // ===== کمک‌کننده‌های Async (بدون تغییر) =====

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
        public T Current => _inner.Current;
        public ValueTask DisposeAsync() => new(Task.CompletedTask);
        public ValueTask<bool> MoveNextAsync() => new(_inner.MoveNext());
    }

    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;
        public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

        public IQueryable CreateQuery(Expression expression)
            => new TestAsyncEnumerable<TEntity>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new TestAsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression) => _inner.Execute(expression);
        public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
            => new TestAsyncEnumerable<TResult>(expression);
        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            => Execute<TResult>(expression);
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }
}
