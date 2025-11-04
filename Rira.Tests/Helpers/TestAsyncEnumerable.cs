using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rira.Tests.Common.Mocks
{
    /// <summary>
    /// ✅ کلاس کمکی برای تبدیل IQueryable به AsyncEnumerable جهت تست‌های EF Core 8
    /// </summary>
    public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        private readonly Expression _expression;

        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable) => _expression = Expression.Constant(this);

        public TestAsyncEnumerable(Expression expression)
            : base(expression) => _expression = expression;

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            // رفع خطای CS0201 — فراخوانی () الزامی شد
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        public IQueryProvider Provider => new TestAsyncQueryProvider(this);

        Expression IQueryable.Expression => _expression;
    }

    /// <summary>
    /// Enumerator Async برای تست‌های EF Core 8
    /// </summary>
    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

        public T Current => _inner.Current;

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return default;
        }

        public ValueTask<bool> MoveNextAsync() =>
            new ValueTask<bool>(_inner.MoveNext());
    }
}
