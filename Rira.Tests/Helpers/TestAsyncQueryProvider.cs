using Microsoft.EntityFrameworkCore.Query;
using RiraApi.Tests.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Rira.Tests.Common.Mocks
{
    /// <summary>
    /// ✅ کلاس Mock برای شبیه‌سازی Provider کوئری‌های Async در EF Core 8
    /// نسخهٔ نهایی غیرجنریک و سازگار با متدهای جدید ExecuteAsyncEnumerable و ExecuteAsync.
    /// </summary>
    public class TestAsyncQueryProvider : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

        public IQueryable CreateQuery(Expression expression) =>
            new TestAsyncEnumerable<object>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
            new TestAsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression) => _inner.Execute(expression);

        public TResult Execute<TResult>(Expression expression)
        {
            try
            {
                // اجرای مستقیم Expression در حافظه جهت رفع ArgumentException
                return Expression.Lambda<Func<TResult>>(expression).Compile().Invoke();
            }
            catch
            {
                return _inner.Execute<TResult>(expression);
            }
        }

        // نسخهٔ هماهنگ با EF 8
        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            // در EF 8 خروجی ValueTask نیست بلکه TResult مستقیم است
            var result = Execute<TResult>(expression);
            return result;
        }

        public IAsyncEnumerable<TResult> ExecuteAsyncEnumerable<TResult>(Expression expression)
        {
            // فراهم‌کردن IEnumerable برای توابع Async LINQ در تست
            var result = Execute<IEnumerable<TResult>>(expression);
            return new TestAsyncEnumerable<TResult>(result);
        }
    }
}
