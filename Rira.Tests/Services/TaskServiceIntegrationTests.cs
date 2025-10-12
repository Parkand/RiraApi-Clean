using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rira.Application.DTOs;
using Rira.Application.Services;
using Rira.Application.Validators;
using Rira.Domain.Entities;
using Rira.Tests.TestUtilities;
using Xunit;

namespace Rira.Tests.Application.Services
{
    /// <summary>
    /// ✅ تست‌های یکپارچگی سرویس TaskService در پروژه ریرا
    /// --------------------------------------------------------------------------------------------------
    /// این تست‌ها منطق واقعی سرویس را با AutoMapper، Validator و DbContext حافظه‌ای بررسی می‌کنند.
    /// </summary>
    public class TaskServiceIntegrationTests
    {
        // 🧩 وابستگی‌ها (Mapper و Validator)
        private readonly IMapper _mapper;
        private readonly TaskDtoValidator _validator;

        // ==========================================================================================
        // ⚙️ سازنده‌ی کلاس تست — نسخه سازگار با AutoMapper v12+
        // ==========================================================================================
        public TaskServiceIntegrationTests()
        {
            // ✅ ایجاد کانتینر DI برای سنجش واقع‌گرایانه ماپر و والیدیتور
            var services = new ServiceCollection();

            // 📦 ثبت پروفایل نگاشت مستقیم بین TaskEntity و TaskDto
            services.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<TaskEntity, TaskDto>().ReverseMap();
            });

            // 📦 ثبت Validator به عنوان سرویس واقعی
            services.AddScoped<TaskDtoValidator>();

            // ⚙️ ساخت ServiceProvider و واکشی Instanceها
            var provider = services.BuildServiceProvider();
            _mapper = provider.GetRequiredService<IMapper>();
            _validator = provider.GetRequiredService<TaskDtoValidator>();
        }

        // ==========================================================================================
        // 🎯 تست ۱: بررسی رفتار سرویس در ثبت تسک معتبر
        // ==========================================================================================
        [Fact(DisplayName = "ایجاد تسک معتبر باید پاسخ موفق برگرداند")]
        public async Task CreateTask_Should_Return_SuccessResponse_For_ValidData()
        {
            var dbContext = InMemoryContextFactory.CreateDbContext();
            var service = new TaskService(dbContext, _mapper, _validator);

            var dto = new TaskDto
            {
                Title = "نوشتن تست یکپارچگی",
                Description = "بررسی رفتار سرویس در حالت داده معتبر",
                Status = "Pending",
                Priority = "High",
                CreatedAt = "1403/01/01",
                UpdatedAt = "1403/01/02"
            };

            var response = await service.CreateTaskAsync(dto);

            response.Should().NotBeNull();
            response.Success.Should().BeTrue("چون داده معتبر است و باید با موفقیت ثبت شود.");
            response.Message.Should().Contain("موفق");
            response.Data.Should().NotBeNull();

            var record = await dbContext.Tasks.FirstOrDefaultAsync();
            record.Should().NotBeNull();
            record.Title.Should().Be(dto.Title);
            record.Status.ToString().Should().Be(dto.Status);
        }

        // ==========================================================================================
        // 🎯 تست ۲: بررسی ایجاد تسک نامعتبر (Validator باید Fail دهد)
        // ==========================================================================================
        [Fact(DisplayName = "ایجاد تسک نامعتبر باید Fail برگرداند")]
        public async Task CreateTask_Should_Return_FailResponse_For_InvalidData()
        {
            var dbContext = InMemoryContextFactory.CreateDbContext();
            var service = new TaskService(dbContext, _mapper, _validator);

            var invalidDto = new TaskDto
            {
                Title = "",                       // ❌ خالی، نقض قاعده NotEmpty()
                Description = "توضیحات تست نامعتبر",
                Status = "WrongStatus",           // ❌ مقدار غیرمجاز Enum
                Priority = "InvalidPriority",     // ❌ مقدار غیرمجاز Enum
                CreatedAt = "2024-01-01"          // ❌ فرمت میلادی به‌جای شمسـی
            };

            var response = await service.CreateTaskAsync(invalidDto);

            response.Should().NotBeNull();
            response.Success.Should().BeFalse("چون داده نامعتبر است.");
            response.Message.Should().Contain("اعتبارسنجی");
            response.Data.Should().BeNull();
        }

        // ==========================================================================================
        // 🎯 تست ۳: واکشی همهٔ تسک‌ها از دیتابیس حافظه‌ای
        // ==========================================================================================
        [Fact(DisplayName = "واکشی همهٔ تسک‌ها باید لیست کامل برگرداند")]
        public async Task GetAllTasks_Should_Return_List_When_Exists()
        {
            var dbContext = InMemoryContextFactory.CreateDbContext();
            dbContext.Tasks.AddRange(
                new TaskEntity
                {
                    Title = "تسک اول",
                    Description = "نمونه ۱",
                    Status = Domain.Entities.TaskStatus.Pending,
                    Priority = TaskPriority.Low
                },
                new TaskEntity
                {
                    Title = "تسک دوم",
                    Description = "نمونه ۲",
                    Status = Domain.Entities.TaskStatus.Completed,
                    Priority = TaskPriority.High
                }
            );
            await dbContext.SaveChangesAsync();

            var service = new TaskService(dbContext, _mapper, _validator);
            var response = await service.GetAllTasksAsync();

            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Data.Should().HaveCount(2);
            response.Data![0].Title.Should().Be("تسک اول");
        }

        // ==========================================================================================
        // 🎯 تست ۴: واکشی تسک موجود بر اساس شناسه
        // ==========================================================================================
        [Fact(DisplayName = "واکشی تسک موجود باید داده صحیح بدهد")]
        public async Task GetTaskById_Should_Return_Task_When_Exists()
        {
            var dbContext = InMemoryContextFactory.CreateDbContext();

            var entity = new TaskEntity
            {
                Title = "تسک تستی موجود",
                Description = "برای بررسی متد واکشی تک‌تسک",
                Status = Domain.Entities.TaskStatus.InProgress,
                Priority = TaskPriority.Medium
            };

            dbContext.Tasks.Add(entity);
            await dbContext.SaveChangesAsync();

            var service = new TaskService(dbContext, _mapper, _validator);
            var response = await service.GetTaskByIdAsync(entity.Id);

            response.Should().NotBeNull();
            response.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data.Title.Should().Be(entity.Title);
            response.Message.Should().Contain("موفق");
        }

        // ==========================================================================================
        // 🎯 تست ۵: واکشی تسک ناموجود (باید خطای یافت‌نشد بدهد)
        // ==========================================================================================
        [Fact(DisplayName = "واکشی تسک ناموجود باید Fail برگرداند")]
        public async Task GetTaskById_Should_Return_Fail_When_NotFound()
        {
            var dbContext = InMemoryContextFactory.CreateDbContext();
            var service = new TaskService(dbContext, _mapper, _validator);

            var response = await service.GetTaskByIdAsync(999);

            response.Should().NotBeNull();
            response.Success.Should().BeFalse("تسکی با این شناسه وجود ندارد و باید خطای NotFound دهد.");
            response.Message.Should().Contain("یافت نشد");
            response.Data.Should().BeNull();
        }
    }
}
