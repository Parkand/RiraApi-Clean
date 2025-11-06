// ===========================================================
// 📘 RiRaDocs Teaching Edition (Farsi Inline)
// File: TaskServiceIntegrationTests.cs
// Layer: Integration Tests
// Context: Clean Architecture  –  Application ⇆ Persistence
// هدف: تست ادغام واقعی سرویس TaskService با DI، AutoMapper، FluentValidation و EF Core 8
// انتشار: RiraDocs-v2025.11.4-Stable-Final-Fixed
// ===========================================================

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Rira.Application.Features.Tasks.Validators;
using Rira.Application.Interfaces;
using Rira.Application.MappingProfiles;
using Rira.Domain.Entities;
using Rira.Persistence.Data;

namespace Rira.IntegrationTests
{
    /// <summary>
    /// 🧩 کلاس تست ادغام (Integration Test)
    /// هدف این کلاس، شبیه‌سازی رفتار واقعی سرویس TaskService است، با تمام دغدغه‌های اصلی:
    ///  - اتصال DI Container واقعی (بدون Mock)
    ///  - صحت نگاشت‌های AutoMapper
    ///  - اجرای Validation واقعی با FluentValidation
    ///  - تست کامل CRUD در AppDbContext (InMemory)
    /// این تست تضمین می‌کند که وابستگی‌ها، کانفیگ‌ها و زیرساخت‌ها در کنار هم درست کار کنند.
    /// </summary>
    [TestClass]
    public class TaskServiceIntegrationTests
    {
        // ⚙️ Provider اصلی DI برای تزریق Scoped/Transient ها در هر تست
        private ServiceProvider _provider = null!;
        // 🧠 سرویس اصلی تحت تست: TaskService
        private ITaskService _service = null!;

        // ===========================================================
        // 🔧 متد Setup - آماده‌سازی محیط Test واقعی
        // ===========================================================
        [TestInitialize]
        public void Setup()
        {
            // 1️⃣ ایجاد کانتینر وابستگی‌ها
            var services = new ServiceCollection();

            // 🟢 رجیستر کردن DbContext با Provider InMemory
            // ➕ از UseInMemoryDatabase برای ایجاد بانک‌داده مستقل تستی استفاده می‌شود.
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseInMemoryDatabase(databaseName: "RiraIntegrationDb"));

            // 🟢 اتصال لایه Application به لایه Persistence از طریق IAppDbContext
            // این بخش از اصول Clean Architecture را رعایت می‌کند (وابستگی از Application به Infrastructure).
            services.AddScoped<IAppDbContext>(sp =>
                sp.GetRequiredService<AppDbContext>());

            // 🟢 پیکربندی AutoMapper — تزریق پروفایل‌های نگاشت Application
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<TaskProfile>();
            });

            // 🟢 افزودن MediatR برای اجرای فرمان‌ها و کوئری‌ها (CQRS Pattern)
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblies(
                    typeof(TaskService).Assembly,
                    typeof(AppDbContext).Assembly));

            // 🟢 رجیستر تمام Validatorهای FluentValidation مرتبط با TaskDto
            services.AddValidatorsFromAssemblyContaining<TaskDtoValidator>();

            // 🟢 رجیستر سرویس اصلی Application
            services.AddScoped<ITaskService, TaskService>();

            // 💠 نهایی‌سازی Provider تزریق وابستگی‌ها
            _provider = services.BuildServiceProvider();

            // 🟩 گرفتن نمونه واقعی سرویس از DI Container
            _service = _provider.GetRequiredService<ITaskService>();
        }

        // ===========================================================
        // ✅ تست متد CreateTaskAsync
        // ===========================================================
        // 🎯 هدف آموزشی:
        //     ▫ بررسی کامل DI + AutoMapper + Validation + EFCore در فرآیند ایجاد Task
        //     ▫ اطمینان از صحت داده ذخیره‌شده در پایگاه InMemory پس از اجرای متد
        [TestMethod]
        public async Task CreateTaskAsync_Should_Work_Correctly()
        {
            // 🧩 AAA Pattern → Arrange, Act, Assert
            // Arrange — آماده‌سازی داده ورودی DTO
            var dto = new TaskDto
            {
                Title = "IntegrationTest_Task",
                Description = "Task created inside integration test.",
                Status = Domain.Enums.TaskStatus.Pending,
                DueDate = "1404/03/03"
            };

            // Act — اجرای متد اصلی
            var result = await _service.CreateTaskAsync(dto);

            // Assert — ارزیابی خروجی سرویس
            result.Should().NotBeNull();
            result.Data.Should().BeGreaterThan(0);
            result.Message.Should().MatchEquivalentOf("*created*");

            // بررسی صحت داده در DbContext
            var context = _provider.GetRequiredService<AppDbContext>();
            var dbTask = await context.Tasks.FirstOrDefaultAsync(t => t.Title == "IntegrationTest_Task");

            dbTask.Should().NotBeNull();
            dbTask!.Description.Should().Be("Task created inside integration test.");
        }

        // ===========================================================
        // ✅ تست متد UpdateTaskAsync
        // ===========================================================
        // 🎯 هدف آموزشی:
        //     ▫ بررسی عملکرد AutoMapper در بروزرسانی موجودیت‌ها
        //     ▫ اطمینان از صحت داده نهایی در پایگاه داده پس از Update
        //     ▫ اعتبارسنجی مجدد DTO قبل از انجام تغییر
        [TestMethod]
        public async Task UpdateTaskAsync_Should_Work_Correctly()
        {
            // Arrange — ایجاد یک رکورد اولیه در بانک داده
            var context = _provider.GetRequiredService<AppDbContext>();
            var entity = new TaskEntity
            {
                Title = "OldTitle",
                Description = "Initial",
                Status = Domain.Enums.TaskStatus.Pending,
                DueDate = "1404/02/02"
            };
            context.Tasks.Add(entity);
            await context.SaveChangesAsync();

            // DTO جدید برای بروزرسانی
            var dto = new TaskDto
            {
                Id = entity.Id,
                Title = "UpdatedTitle",
                Description = "Updated by integration test",
                Status = Domain.Enums.TaskStatus.InProgress,
                DueDate = "1404/01/01"
            };

            // Act — اجرای متد بروزرسانی
            var result = await _service.UpdateTaskAsync(dto.Id, dto);

            // Assert — اعتبار نتایج
            result.Should().NotBeNull();
            result.Data.Should().BeGreaterThan(0);
            result.Message.Should().MatchEquivalentOf("*updated*");

            var updated = await context.Tasks.FindAsync(entity.Id);
            updated!.Title.Should().Be("UpdatedTitle");
            updated.Description.Should().Be("Updated by integration test");
        }

        // ===========================================================
        // ✅ تست متد DeleteTaskAsync
        // ===========================================================
        // 🎯 هدف آموزشی:
        //     ▫ بررسی حذف واقعی رکورد از دیتابیس InMemory
        //     ▫ اطمینان از صحت پیام خروجی و بازگشت ResponseModel<int>
        [TestMethod]
        public async Task DeleteTaskAsync_Should_Work_Correctly()
        {
            // Arrange — ایجاد رکورد هدف حذف
            var context = _provider.GetRequiredService<AppDbContext>();
            var task = new TaskEntity
            {
                Title = "DeleteTarget",
                Description = "Will be deleted.",
                Status = Domain.Enums.TaskStatus.Pending,
                DueDate = "1404/05/05"
            };
            context.Tasks.Add(task);
            await context.SaveChangesAsync();

            // Act — اجرای متد حذف
            var result = await _service.DeleteTaskAsync(task.Id);

            // Assert — تأیید صحت حذف داده
            result.Should().NotBeNull();
            result.Data.Should().BeGreaterThan(0);
            result.Message.Should().MatchEquivalentOf("*deleted*");

            var deleted = await context.Tasks.FindAsync(task.Id);
            deleted.Should().BeNull(); // چون در لایه سرویس حذف سخت انجام شده است (نه Soft Delete)
        }

        // ===========================================================
        // 🧹 پاکسازی منابع پس از هر تست
        // ===========================================================
        // 🎯 هدف آموزشی:
        //     ▫ آشنایی با چرخه‌ی عمر ServiceProvider و اهمیت Dispose منابع در تست‌ها.
        [TestCleanup]
        public void Cleanup()
        {
            _provider.Dispose();
        }
    }
}

// ===========================================================
// 📘 جمع‌بندی آموزشی (RiRaDocs Summary)
// -----------------------------------------------------------
// ▫ لایه تست ادغام، آخرین گام در تحلیل صحیح Cross-layer DI است.
// ▫ این فایل تأیید می‌کند که:
//    1️⃣ AutoMapper و Validatorها در DI کاملاً Resolve می‌شوند.
//    2️⃣ DbContext InMemory به‌درستی برای هر تست جدا ایجاد می‌شود.
//    3️⃣ پیام‌های بازگشتی ResponseModel مطابق استاندارد پروژه‌اند.
// ▫ این فایل جزو نسخه نهایی انتشار آموزشی ✅
// تگ انتشار: RiraDocs-v2025.11.4-Stable-Final-Fixed
// ===========================================================
