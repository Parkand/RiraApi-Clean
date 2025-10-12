using AutoMapper;
using FluentAssertions;
using Rira.Application.DTOs;
using Rira.Application.Services;
using Rira.Application.Validators;
using Rira.Domain.Entities;
using Rira.Tests.TestUtilities;
using Xunit;

namespace Rira.Tests.Services
{
    /// <summary>
    /// ✅ تست واحد سرویس TaskService
    /// در این کلاس، تمام متدهای Create, Read, Update, Delete سرویس تست می‌شوند.
    /// هر تست خروجی را بر اساس مدل استاندارد ریرا یعنی ResponseModel<T> بررسی می‌کند.
    /// </summary>
    public class TaskServiceTests
    {
        private readonly IMapper _mapper;
        private readonly TaskDtoValidator _validator;

        // ==========================================================================================
        // ⚙️ سازنده‌ی تست واحد (Unit Test Constructor)
        // AutoMapper و Validator با تنظیمات واقعی پروژه ریرا مقداردهی می‌شوند.
        //
        // 💡 نکته AutoMapper v12+:
        // برای جلوگیری از خطای CS1729 در نسخه جدید، پارامتر دوم سازنده‌ی MapperConfiguration
        // (ILoggerFactory) به صورت null ارسال می‌شود.
        // ==========================================================================================
        public TaskServiceTests()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TaskDto, TaskEntity>().ReverseMap();
            }, null); // پارامتر دوم null برای سازگاری با AutoMapper v12+

            _mapper = configuration.CreateMapper();
            _validator = new TaskDtoValidator();
        }

        // ==========================================================================================
        // ✅ تست ۱: ایجاد تسک جدید
        // ==========================================================================================
        [Fact]
        public async Task CreateTaskAsync_Should_Save_Task_Correctly()
        {
            var context = InMemoryContextFactory.CreateDbContext();
            var service = new TaskService(context, _mapper, _validator);

            var dto = new TaskDto
            {
                Title = "نوشتن تست واحد سرویس",
                Description = "هدف: ارزیابی عملکرد CreateTaskAsync",
                Status = "Pending",
                Priority = "High",
                DueDate = "1404/07/20"
            };

            var result = await service.CreateTaskAsync(dto);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Title.Should().Be(dto.Title);

            var all = await service.GetAllTasksAsync();
            all.Data.Should().HaveCount(1);
        }

        // ==========================================================================================
        // ✅ تست ۲: بروزرسانی تسک موجود و تغییر وضعیت
        // ------------------------------------------------------------------------------------------
        // فراخوانی متد UpdateTaskAsync اکنون با پارامتر شناسه (int id) انجام می‌شود.
        // ==========================================================================================
        [Fact]
        public async Task UpdateTaskAsync_Should_Change_Status_To_Completed()
        {
            var context = InMemoryContextFactory.CreateDbContext();
            var service = new TaskService(context, _mapper, _validator);

            var dto = new TaskDto
            {
                Title = "تسک نمونه جهت بروزرسانی",
                Description = "در حال انجام کار",
                Status = "InProgress",
                Priority = "Medium",
                DueDate = "1404/07/15"
            };

            var created = await service.CreateTaskAsync(dto);

            var updatedDto = new TaskDto
            {
                Id = created.Data.Id,
                Title = dto.Title,
                Description = "اکنون تکمیل شده",
                Status = "Completed",
                Priority = "Medium",
                DueDate = dto.DueDate
            };

            // ✅ پارامتر شناسه جداگانه ارسال می‌شود
            var updated = await service.UpdateTaskAsync(created.Data.Id, updatedDto);

            updated.Success.Should().BeTrue();
            updated.Data.Should().NotBeNull();
            updated.Data.Status.Should().Be("Completed");
        }

        // ==========================================================================================
        // ✅ تست ۳: حذف تسک (Hard Delete)
        // ==========================================================================================
        [Fact]
        public async Task DeleteTaskAsync_Should_Remove_Task()
        {
            var context = InMemoryContextFactory.CreateDbContext();
            var service = new TaskService(context, _mapper, _validator);

            var dto = new TaskDto
            {
                Title = "تسک برای حذف",
                Status = "Pending",
                Priority = "Low",
                DueDate = "1404/07/22"
            };

            var created = await service.CreateTaskAsync(dto);
            var result = await service.DeleteTaskAsync(created.Data.Id);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();

            var all = await service.GetAllTasksAsync();
            all.Data.Should().BeEmpty();
        }

        // ==========================================================================================
        // ✅ تست ۴: واکشی همهٔ تسک‌ها
        // ==========================================================================================
        [Fact]
        public async Task GetAllTasksAsync_Should_Return_All_Tasks()
        {
            var context = InMemoryContextFactory.CreateDbContext();
            var service = new TaskService(context, _mapper, _validator);

            await service.CreateTaskAsync(new TaskDto
            {
                Title = "تسک اول",
                Status = "Pending",
                Priority = "High",
                DueDate = "1404/07/23"
            });

            await service.CreateTaskAsync(new TaskDto
            {
                Title = "تسک دوم",
                Status = "Completed",
                Priority = "Medium",
                DueDate = "1404/07/24"
            });

            var result = await service.GetAllTasksAsync();
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().HaveCount(2);
        }

        // ==========================================================================================
        // ✅ تست ۵: دریافت تسک بر اساس شناسه
        // ==========================================================================================
        [Fact]
        public async Task GetTaskByIdAsync_Should_Return_Correct_Task()
        {
            var context = InMemoryContextFactory.CreateDbContext();
            var service = new TaskService(context, _mapper, _validator);

            var dto = new TaskDto
            {
                Title = "تسک تست شناسه",
                Description = "هدف: آزمون GetTaskByIdAsync",
                Status = "Pending",
                Priority = "Critical",
                DueDate = "1404/07/25"
            };

            var created = await service.CreateTaskAsync(dto);
            var found = await service.GetTaskByIdAsync(created.Data.Id);

            found.Should().NotBeNull();
            found.Success.Should().BeTrue();
            found.Data.Title.Should().Be(dto.Title);
        }
    }
}
