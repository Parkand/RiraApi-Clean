using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Rira.Application.Features.Tasks.Commands.Create;
using Rira.Application.Interfaces;
using Rira.Application.MappingProfiles;
using Rira.Application.Validators;
using Rira.Domain.Entities;
using Rira.Domain.Enums;
using Rira.Persistence.Data;
using System.Net;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Rira.Application.DTOs; // برای TaskDto
using FluentAssertions;
using MediatR;
using TaskStatus = Rira.Domain.Enums.TaskStatus;

namespace Rira.Tests.Application.Services
{
    public class TaskServiceIntegrationTests
    {
        private readonly ITaskService _taskService;
        private readonly IAppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ServiceProvider _serviceProvider;

        public TaskServiceIntegrationTests()
        {
            var services = new ServiceCollection();

            // ✅ پیکربندی DbContext با InMemory
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("RiraTestDb"));

            // ✅ AutoMapper با پروفایل واقعی
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<TaskProfile>();
            });

            // ✅ MediatR برای Command/Queryها
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(TaskCreateCommandHandler).Assembly));

            // ✅ Validatorهای FluentValidation
            services.AddValidatorsFromAssemblyContaining<TaskDtoValidator>();

            // ✅ ثبت سرویس TaskService
            services.AddScoped<ITaskService, TaskService>();

            _serviceProvider = services.BuildServiceProvider();

            _context = _serviceProvider.GetRequiredService<AppDbContext>();
            _mapper = _serviceProvider.GetRequiredService<IMapper>();
            _taskService = _serviceProvider.GetRequiredService<ITaskService>();

            SeedTestDataAsync().GetAwaiter().GetResult();
        }

        private async Task SeedTestDataAsync()
        {
            _context.Tasks.RemoveRange(_context.Tasks);
            await _context.SaveChangesAsync();

            var tasks = new List<TaskEntity>
            {
                new TaskEntity { Id = 1, Title = "Task 1", Description = "Desc 1", Status = TaskStatus.Pending, Priority = TaskPriority.Medium },
                new TaskEntity { Id = 2, Title = "Task 2", Description = "Desc 2", Status = TaskStatus.InProgress, Priority = TaskPriority.High }
            };
            _context.Tasks.AddRange(tasks);
            await _context.SaveChangesAsync();
        }

        // 🚀 تست ایجاد تسک جدید
        [Fact]
        public async Task CreateTaskAsync_Should_Save_Task_Correctly()
        {
            var dto = new TaskDto
            {
                Title = "New Task",
                Description = "Integration Test Task",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Low
            };

            var result = await _taskService.CreateTaskAsync(dto);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // ✅ چون خروجی ResponseModel<int> است
            result.Data.Should().BeOfType(typeof(int));
            ((int)result.Data).Should().BeGreaterThan(0);

            var createdTask = await _context.Tasks.FindAsync(result.Data);
            createdTask.Should().NotBeNull();
            createdTask!.Title.Should().Be(dto.Title);
        }

        // 🚀 تست واکشی همه تسک‌ها
        [Fact]
        public async Task GetAllTasksAsync_Should_Return_All_Tasks()
        {
            var result = await _taskService.GetAllTasksAsync();

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // ✅ چون Data از نوع List<TaskDto> است
            result.Data.Should().NotBeNull();
            result.Data.Should().NotBeEmpty();
            result.Data.Count.Should().BeGreaterThanOrEqualTo(2);
        }

        // 🚀 تست واکشی تسک بر اساس Id
        [Fact]
        public async Task GetTaskByIdAsync_Should_Return_Correct_Task()
        {
            var result = await _taskService.GetTaskByIdAsync(1);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            result.Data.Should().NotBeNull();
            result.Data.Title.Should().Be("Task 1");
            result.Data.Status.Should().Be(TaskStatus.Pending);
            result.Data.Priority.Should().Be(TaskPriority.Medium);
        }

        // 🚀 تست واکشی تسک که وجود ندارد
        [Fact]
        public async Task GetTaskByIdAsync_Should_Fail_When_NotFound()
        {
            var result = await _taskService.GetTaskByIdAsync(9999);

            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        // 🚀 تست حذف تسک (Soft Delete)
        [Fact]
        public async Task DeleteTaskAsync_Should_SoftDelete_Task()
        {
            var result = await _taskService.DeleteTaskAsync(2);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            var deleted = await _context.Tasks.FindAsync(2);
            deleted.Should().NotBeNull();
            deleted!.IsDeleted.Should().BeTrue(); // ✅ منطق Soft Delete
        }

        // 🚀 تست بروزرسانی وضعیت
        [Fact]
        public async Task UpdateTaskAsync_Should_Change_Status_To_Completed()
        {
            var dto = new TaskDto
            {
                Id = 1,
                Title = "Task 1 Updated",
                Description = "Updated Desc",
                Status = TaskStatus.Completed,
                Priority = TaskPriority.High
            };

            // ابتدا ایجاد تسک جدید
            var created = await _taskService.CreateTaskAsync(dto);

            // سپس بروزرسانی با id و dto جدید
            var updatedDto = new TaskDto
            {
                Title = "ویرایش‌شده",
                Description = "تست بروزرسانی",
                Status = TaskStatus.Completed,
                Priority = TaskPriority.Medium,
                DueDate = "1404/07/25"
            };

            var result = await _taskService.UpdateTaskAsync(created.Data, updatedDto);


            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);

            // ✅ خروجی ResponseModel<int>
            result.Data.Should().BeOfType(typeof(int));
            ((int)result.Data).Should().BeGreaterThan(0);

            var updatedTask = await _context.Tasks.FindAsync(1);
            updatedTask.Should().NotBeNull();
            updatedTask!.Status.Should().Be(TaskStatus.Completed);
            updatedTask.Title.Should().Be("Task 1 Updated");
        }
    }
}
