// ===========================================================
// RiraDocs 🧩
// File: TaskServiceIntegrationTests.cs
// Version: 2025/10/30
// Context: Clean Architecture - Integration Tests Layer
// هدف: اجرای تست‌های ادغام (Integration) برای TaskService
// وضعیت: نهایی، بدون خطای AutoMapper و DI
// ===========================================================

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Rira.Application.Interfaces;
using Rira.Application.MappingProfiles;
using Rira.Application.Validators;
using Rira.Domain.Entities;
using Rira.Persistence.Data;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

namespace Rira.IntegrationTests
{
    [TestClass]
    public class TaskServiceIntegrationTests
    {
        // ⚙️ Provider اصلی DI
        private ServiceProvider _provider = null!;
        private ITaskService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            // 1️⃣ تنظیم کانتینر DI
            var services = new ServiceCollection();

            // 🟢 رجیستری DbContext InMemory مخصوص تست‌های ادغام
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseInMemoryDatabase(databaseName: "RiraIntegrationDb"));

            // 🟢 اتصال واسط Application به DbContext اصلی
            // (این خط کلیدی‌ترین بخش برای رفع InvalidOperationException است)
            services.AddScoped<IAppDbContext>(sp =>
                sp.GetRequiredService<AppDbContext>());

            // 🟢 رجیستری AutoMapper (نسخه 15.0.1 ✅ از قبل تأییدشده)
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<TaskProfile>();
            });


            // 🟢 MediatR برای Command/Query Handlerها
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblies(
                    typeof(TaskService).Assembly,
                    typeof(AppDbContext).Assembly));

            // 🟢 Validatorها برای DTOها (FluentValidation)
            services.AddValidatorsFromAssemblyContaining<TaskDtoValidator>();

            // 🟢 سرویس اصلی Application
            services.AddScoped<ITaskService, TaskService>();

            // 💠 ساختن Provider نهایی
            _provider = services.BuildServiceProvider();

            // 🟩 گرفتن سرویس TaskService واقعی (با DI کامل)
            _service = _provider.GetRequiredService<ITaskService>();
        }

        // ===========================================================
        // ✅ تست نمونه برای متد CreateTaskAsync
        // ===========================================================
        [TestMethod]
        public async Task CreateTaskAsync_Should_Work_Correctly()
        {
            // AAA Pattern
            // Arrange
            var dto = new TaskDto
            {
                Title = "IntegrationTest_Task",
                Description = "Task created inside integration test.",
                Status = Domain.Enums.TaskStatus.Pending,
                DueDate = "1404/03/03"
            };

            // Act
            var result = await _service.CreateTaskAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeGreaterThan(0); // اطمینان از ساخت موفق
            result.Message.Should().MatchEquivalentOf("*created*");

            // ⏹ بررسی صحت درون پایگاه داده
            var context = _provider.GetRequiredService<AppDbContext>();
            var dbTask = await context.Tasks.FirstOrDefaultAsync(t => t.Title == "IntegrationTest_Task");

            dbTask.Should().NotBeNull();
            dbTask!.Description.Should().Be("Task created inside integration test.");
        }

        // ===========================================================
        // ✅ تست نمونه برای متد UpdateTaskAsync
        // ===========================================================
        [TestMethod]
        public async Task UpdateTaskAsync_Should_Work_Correctly()
        {
            // Arrange
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

            var dto = new TaskDto
            {
                Id = entity.Id,
                Title = "UpdatedTitle",
                Description = "Updated by integration test",
                Status = Domain.Enums.TaskStatus.InProgress,
                DueDate = "1404/01/01"
            };

            // Act
            var result = await _service.UpdateTaskAsync(dto.Id, dto);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeGreaterThan(0); // چون Update هم ResponseModel<int> برمی‌گردونه
            result.Message.Should().MatchEquivalentOf("*updated*");

            // بررسی پایگاه داده
            var updated = await context.Tasks.FindAsync(entity.Id);
            updated!.Title.Should().Be("UpdatedTitle");
            updated.Description.Should().Be("Updated by integration test");
        }

        // ===========================================================
        // ✅ تست نمونه برای حذف (DeleteTaskAsync)
        // ===========================================================
        [TestMethod]
        public async Task DeleteTaskAsync_Should_Work_Correctly()
        {
            // Arrange
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

            // Act
            var result = await _service.DeleteTaskAsync(task.Id);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().BeGreaterThan(0);
            result.Message.Should().MatchEquivalentOf("*deleted*");

            var deleted = await context.Tasks.FindAsync(task.Id);
            deleted.Should().BeNull();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _provider.Dispose();
        }
    }
}
