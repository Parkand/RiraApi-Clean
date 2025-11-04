using Microsoft.EntityFrameworkCore.Query;
using RiraApi.Tests.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rira.Tests.Common.Mocks
{
    // 🧩 TestAsyncQueryProvider
    // ===============================================================================
    // کلاس Provider اصلی برای شبیه‌سازی کوئری‌های Async در EF Core 8 در محیط تست.
    // این کلاس جایگزین Provider واقعی EF است و امکان اجرای متدهای غیرهم‌زمان
    // مانند ToListAsync(), FirstAsync(), CountAsync() و سایر LINQ‌های Async را
    // در DbSetهای Mock فراهم می‌کند.
    //
    // 🎯 اهداف آموزشی RiRaDocs:
    //     ▫ درک مفهوم IAsyncQueryProvider و نقش آن در EF Core.
    //     ▫ نحوه‌ی نگاشت Expression Tree به پرس‌وجوهای حافظه‌ای در تست.
    //     ▫ آموزش تفاوت اجرای Async در EF Core 8 نسبت به نسخه‌های قبل.
    //
    // 🔧 ویژگی‌های نسخه نهایی:
    //     ▫ سازگار با EF Core 8 (به‌جای ValueTask<TResult> اکنون TResult مستقیم بازگردانده می‌شود)
    //     ▫ پشتیبانی از ExecuteAsyncEnumerable<TResult> جهت شبیه‌سازی حلقه‌های await‑foreach
    //     ▫ مدیریت خطا برای جلوگیری از ArgumentException هنگام Compile Expression
    // ===============================================================================
    public class TestAsyncQueryProvider : IAsyncQueryProvider
    {
        // ---------------------------------------------------------------------------
        // 🧱 فیلد داخلی: Provider واقعی EF یا Linq
        // ---------------------------------------------------------------------------
        private readonly IQueryProvider _inner;

        // ---------------------------------------------------------------------------
        // 🟢 سازنده
        // ---------------------------------------------------------------------------
        // Provider اصلی را دریافت کرده و برای استفاده در زمان اجرای Expressionها ذخیره می‌کند.
        public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

        // ---------------------------------------------------------------------------
        // 🔁 CreateQuery(Expression)
        // ---------------------------------------------------------------------------
        // تولید Query غیرجنریک برای انواع ناشناخته در تست‌ها.
        public IQueryable CreateQuery(Expression expression) =>
            new TestAsyncEnumerable<object>(expression);

        // ---------------------------------------------------------------------------
        // 🧩 CreateQuery<TElement>(Expression)
        // ---------------------------------------------------------------------------
        // ایجاد نمونه‌ی IQueryable<T> که قابلیت اجرای Async داشته باشد.
        // خروجی این متد معمولاً در EFAsyncMockHelper هنگام ساخت DbSet مجازی استفاده می‌شود.
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
            new TestAsyncEnumerable<TElement>(expression);

        // ---------------------------------------------------------------------------
        // ⚙️ Execute(Expression)
        // ---------------------------------------------------------------------------
        // اجرای Expression به‌صورت هم‌زمان (Sync).
        // این متد برای متدهای LINQ معمولی (مثل Count یا Any) در حالت تست استفاده می‌شود.
        public object Execute(Expression expression) => _inner.Execute(expression);

        // ---------------------------------------------------------------------------
        // ⚙️ Execute<TResult>(Expression)
        // ---------------------------------------------------------------------------
        // اجرای پرس‌وجوهای با خروجی خاص (جنریک) در حافظه‌ی Mock.
        // در حالت تست ممکن است EF نتواند Expression را مستقیماً کامپایل کند،
        // لذا ابتدا تلاش می‌شود Expression در حافظه کامپایل و invoke شود.
        // در صورت خطا از Provider داخلی استفاده می‌شود.
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

        // ---------------------------------------------------------------------------
        // ⚡ ExecuteAsync<TResult>(Expression, CancellationToken)
        // ---------------------------------------------------------------------------
        // نسخه‌ی هماهنگ با EF Core 8.
        // در EF8 نوع خروجی دیگر ValueTask<TResult> نیست بلکه TResult مستقیماً بازگردانده می‌شود.
        // این متد برای متدهای Async منتهی به تک‌مقدار (مثل FirstAsync) استفاده می‌شود.
        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var result = Execute<TResult>(expression);
            return result;
        }

        // ---------------------------------------------------------------------------
        // 🔄 ExecuteAsyncEnumerable<TResult>(Expression)
        // ---------------------------------------------------------------------------
        // فراهم‌سازی IEnumerable به صورت Async برای توابعی مانند ToListAsync() یا
        // حلقه‌های await‑foreach در تست‌های واحد.
        // داده‌ها به کمک TestAsyncEnumerable<TResult> در حافظه پیمایش می‌شوند.
        public IAsyncEnumerable<TResult> ExecuteAsyncEnumerable<TResult>(Expression expression)
        {
            var result = Execute<IEnumerable<TResult>>(expression);
            return new TestAsyncEnumerable<TResult>(result);
        }
    }

    // ===============================================================================
    // 🧾 خلاصه RiRaDocs Teaching Edition
    // ------------------------------------------------------------------------------
    // 🔹 نقش آموزشی:
    //     ▫ آشنایی عملی با رابط IAsyncQueryProvider و ساخت Providerهای تستی.
    //     ▫ تفاوت‌های کلیدی ExecuteAsync بین EF Core 6/7 و EF Core 8.
    //
    // 🔹 ترکیب در زیرسیستم تست RiraApi:
    //     ▫ با کلاس‌های TestAsyncEnumerable<T> و TestAsyncEnumerator<T> استفاده می‌شود.
    //     ▫ در EFAsyncMockHelper داخل متد BuildMockDbSet<TEntity> تزریق می‌شود.
    //
    // 🔹 نکات EF Core 8:
    //     ▫ حذف ValueTask و استفاده از TResult مستقیم در ExecuteAsync.
    //     ▫ پشتیبانی داخلی از ExecuteAsyncEnumerable برای بازگشت داده‌ها در حالت Stream.
    //
    // 🔹 تگ انتشار RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===============================================================================
}
