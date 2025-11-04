using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rira.Tests.Common.Mocks
{
    // 🧩 TestAsyncEnumerable<T>
    // ===============================================================================
    // این کلاس کمکی، رابط IQueryable<T> را به نسخه‌ی Async قابل‌استفاده در EF Core 8 تبدیل می‌کند.
    // EF Core در متدهای ToListAsync ،FirstOrDefaultAsync ،CountAsync و ...
    // از IAsyncQueryProvider و IAsyncEnumerable استفاده می‌کند تا داده‌ها را به‌صورت غیرهم‌زمان واکشی کند.
    //
    // اما در محیط Mock (مثلاً داخل UnitTest بدون دیتابیس واقعی)، EF چنین Providerی ندارد.
    // در نتیجه برای شبیه‌سازی QueryProvider نیاز به این کلاس است.
    //
    // 🎯 اهداف آموزشی RiRaDocs:
    //     ▫ درک مفهوم Async LINQ Querying در EF Core.
    //     ▫ شبیه‌سازی IQueryable به صورت غیردیتابیسی.
    //     ▫ استفاده در EFAsyncMockHelper برای ایجاد DbSet مجازی.
    // ===============================================================================
    public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        private readonly Expression _expression;

        // ---------------------------------------------------------------------------
        // 🟢 سازنده‌ی مبتنی بر IEnumerable
        // ---------------------------------------------------------------------------
        // این نسخه با دریافت لیست داده‌های تست (مثلاً List<TaskEntity>)
        // شیء IQueryable Async بر پایه‌ی آن ایجاد می‌کند.
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable) => _expression = Expression.Constant(this);

        // ---------------------------------------------------------------------------
        // 🟣 سازنده‌ی مبتنی بر Expression
        // ---------------------------------------------------------------------------
        // در مواقعی که پرس‌وجوهای LINQ تودرتو اجرا شوند، EF عبارت (Expression Tree)
        // را به این سازنده پاس می‌دهد تا QueryProvider جدید تولید شود.
        public TestAsyncEnumerable(Expression expression)
            : base(expression) => _expression = expression;

        // ---------------------------------------------------------------------------
        // 🔁 پیاده‌سازی IAsyncEnumerable<T>
        // ---------------------------------------------------------------------------
        // در EF Core 8 جهت اجرای متدهایی چون ToListAsync، از GetAsyncEnumerator استفاده می‌شود.
        // متد زیر با بازگرداندن شیء TestAsyncEnumerator، امکان پیمایش async را فراهم می‌سازد.
        //
        // ✅ نکته: در EF8، متد باید به صورت () فراخوانی شود (تغییر نسبت به EF6/7).
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        // ---------------------------------------------------------------------------
        // ⚙️ Provider (IQueryProvider)
        // ---------------------------------------------------------------------------
        // برگرداننده‌ی یک QueryProvider سفارشی از نوع TestAsyncQueryProvider
        // که مسئول اجرای Queryهای LINQ به‌صورت Async در محیط Mock است.
        public IQueryProvider Provider => new TestAsyncQueryProvider(this);

        // ---------------------------------------------------------------------------
        // 🧾 Expression
        // ---------------------------------------------------------------------------
        // همان Expression درختی LINQ (مثلاً: tasks.Where(x => x.Id == 1))
        Expression IQueryable.Expression => _expression;
    }

    // 🧭 TestAsyncEnumerator<T>
    // ===============================================================================
    // این کلاس پیمایشگر Async را شبیه‌سازی می‌کند و در اجرای حلقه‌های async
    // مانند await foreach (var item in query) {...} استفاده می‌شود.
    //
    // 🎯 اهداف آموزشی RiRaDocs:
    //     ▫ فهم ساختار Async Enumeration در EF Core.
    //     ▫ آموزش نحوه تعامل Enumerator داخلی در mock‌های EF.
    // ===============================================================================
    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        // ---------------------------------------------------------------------------
        // سازنده: دریافت enumerator واقعی جهت پیمایش در محیط حافظه
        // ---------------------------------------------------------------------------
        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

        // ---------------------------------------------------------------------------
        // پراپرتی Current: آیتم جاری در حلقه‌ی async
        // ---------------------------------------------------------------------------
        public T Current => _inner.Current;

        // ---------------------------------------------------------------------------
        // DisposeAsync: آزادسازی منابع پس از اتمام حلقه async
        // ---------------------------------------------------------------------------
        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return default;
        }

        // ---------------------------------------------------------------------------
        // MoveNextAsync: حرکت به عنصر بعدی در حالت غیرهم‌زمان
        // ---------------------------------------------------------------------------
        // در واقع همان MoveNext هم‌زمان IEnumerable را درون ValueTask قرار می‌دهد.
        public ValueTask<bool> MoveNextAsync() =>
            new ValueTask<bool>(_inner.MoveNext());
    }

    // ===============================================================================
    // 📘 جمع‌بندی RiRaDocs Teaching Edition
    // ------------------------------------------------------------------------------
    // 🔹 نقش کلاس‌ها:
    //     ▫ TestAsyncEnumerable<T> → ایجاد Wrapper برای Queryable داده‌ها
    //     ▫ TestAsyncEnumerator<T> → پشتیبانی از حلقه‌های async و LINQ Async
    //
    // 🔹 نحوه استفاده:
    //     ترکیب این دو کلاس در EFAsyncMockHelper باعث می‌شود EF بتواند روی Datasetهای
    //     تستی (List<TEntity>) متدهای ToListAsync(), FindAsync(), CountAsync() را بدون
    //     دیتابیس اجرا کند.
    //
    // 🔹 نکات EF Core 8:
    //     ▫ الگوی جدید ValueTask در Enumerator و AddAsync.
    //     ▫ نیاز به پیاده‌سازی صریح GetAsyncEnumerator().
    //
    // 🔹 تگ انتشار RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===============================================================================
}
