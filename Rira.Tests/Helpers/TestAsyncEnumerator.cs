using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RiraApi.Tests.Helpers
{
    /// <summary>
    /// Enumerator Async برای پیمایش داده‌های موک در EF Core 8.
    /// </summary>
    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;
        public T Current => _inner.Current;
        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }
        public ValueTask<bool> MoveNextAsync() =>
            new ValueTask<bool>(_inner.MoveNext());
    }
}
