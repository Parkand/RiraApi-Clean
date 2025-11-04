using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RiraApi.Tests.Helpers
{
    // 🧩 TestAsyncEnumerator<T>
    // ===============================================================================
    // این کلاس یکی از اجزای زیرسیستم Mock‑EF Core است که در تست‌های واحد RiraApi به‌کار می‌رود.
    // نقش آن شبیه‌سازی رفتار "Enumerator غیرهم‌زمان" (IAsyncEnumerator<T>) در EF Core 8 است؛
    // یعنی حلقه‌های  await‑foreach  و متدهای LINQ‑Async (مانند ToListAsync، FirstOrDefaultAsync)
    // بتوانند بر روی داده‌های حافظه‌ای (Mocked DbSet) اجرا شوند، بدون نیاز به دیتابیس واقعی.
    //
    // 🎯 اهداف آموزشی RiRaDocs:
    //     ▫ درک نحوه‌ی کار IAsyncEnumerator<T> و تعامل آن با IQueryable mock شده.
    //     ▫ آشنایی با روش تبدیل IEnumerable به نسخه‌ی غیر‌هم‌زمان برای EF Core.
    //     ▫ آموزش الگوی DisposeAsync و ValueTask برای پردازش سبک در تست‌ها.
    //
    // 💡 این کلاس معمولاً در فایل‌هایی مانند "EFAsyncMockHelper.cs" یا "TestAsyncEnumerable.cs"
    // با همان الگوی EF Core 8 به کار گرفته می‌شود.
    // ===============================================================================
    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        // ---------------------------------------------------------------------------
        // 🔹 Enumerator داخلی (نسخه هم‌زمان)
        // ---------------------------------------------------------------------------
        // این فیلد ارجاعی به enumerator واقعی دارد که روی داده‌های حافظه (List<T>) کار می‌کند.
        private readonly IEnumerator<T> _inner;

        // ---------------------------------------------------------------------------
        // 🟢 سازنده
        // ---------------------------------------------------------------------------
        // داده تکرارشونده را دریافت کرده و آن را در _inner ذخیره می‌کند تا در حلقه Async استفاده شود.
        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

        // ---------------------------------------------------------------------------
        // 📦 پراپرتی Current
        // ---------------------------------------------------------------------------
        // آیتم جاری در حلقه  await‑foreach  یا متد Linq Async.
        public T Current => _inner.Current;

        // ---------------------------------------------------------------------------
        // 🧹 DisposeAsync()
        // ---------------------------------------------------------------------------
        // آزادسازی منابع پس از پایان پیمایش.
        // در اینجا فقط Enumerator داخلی Dispose می‌شود.
        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        // ---------------------------------------------------------------------------
        // 🔁 MoveNextAsync()
        // ---------------------------------------------------------------------------
        // وظیفه‌ی حرکت به عنصر بعدی داده را دارد (غیر‌هم‌زمان).
        // در EF Core 8 از ValueTask<bool> برای کاهش سربار استفاده می‌شود.
        public ValueTask<bool> MoveNextAsync() =>
            new ValueTask<bool>(_inner.MoveNext());
    }

    // ===============================================================================
    // 📘 خلاصه آموزشی RiRaDocs
    // ------------------------------------------------------------------------------
    // 🔹 نقش آموزشی:
    //     ▫ آموزش پایه پیمایش غیر‌هم‌زمان در EF Core و شبیه‌سازی رفتار AsyncEnumerator.
    //     ▫ پیاده‌سازی کامل الگوی IAsyncEnumerator<T> برای Mock DbSet.
    //
    // 🔹 نکات فنی EF Core 8:
    //     ▫ از ValueTask به‌جای Task استفاده می‌کند (بهبود کارایی و کاهش GC pressure).
    //     ▫ DisposeAsync همان IDisposable سنتی را پوشش می‌دهد اما به‌صورت awaitable.
    //
    // 🔹 موارد استفاده:
    //     ▫ بخش EFAsyncMockHelper.BuildMockDbSet<TEntity> هنگام ساخت Mock از DbSet.
    //     ▫ پشتیبانی از متدهای غیر‌هم‌زمان در تست‌ها: ToListAsync، CountAsync، FirstOrDefaultAsync و ...
    //
    // 🔹 تگ انتشار RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===============================================================================
}
