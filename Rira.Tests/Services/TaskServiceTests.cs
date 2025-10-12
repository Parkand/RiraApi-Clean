using AutoMapper;
using FluentAssertions;
using Rira.Application.DTOs;
using Rira.Application.Services;
using Rira.Application.Validators;
using Rira.Domain.Entities;
using Rira.Domain.Enums;
using Rira.Tests.TestUtilities;
using Xunit;
using TaskStatus = Rira.Domain.Enums.TaskStatus;

/// <summary>
/// تست واحد برای کلاس TaskService مطابق با استاندارد نهایی ResponseModel<int>
/// </summary>
public class TaskServiceTests
{
    private readonly IMapper _mapper;
    private readonly TaskDtoValidator _validator;

    public TaskServiceTests()
    {
        // تنظیم AutoMapper با سازنده‌ی جدید MapperConfigurationExpression
        var configExpr = new MapperConfigurationExpression();
        configExpr.CreateMap<TaskDto, TaskEntity>().ReverseMap();

        var config = new MapperConfiguration(configExpr, null);
        _mapper = config.CreateMapper();
        _validator = new TaskDtoValidator();
    }

    // ✅ تست ایجاد تسک جدید
    [Fact]
    public async Task CreateTaskAsync_Should_Create_New_Task()
    {
        var context = InMemoryContextFactory.CreateDbContext();
        var service = new TaskService(context, _mapper, _validator);

        var dto = new TaskDto
        {
            Title = "نوشتن تست واحد",
            Description = "تست ایجاد تسک",
            Status = TaskStatus.Pending,
            Priority = TaskPriority.High,
            DueDate = "1404/07/20"
        };

        var result = await service.CreateTaskAsync(dto);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeOfType(typeof(int));
        ((int)result.Data).Should().BeGreaterThan(0);

        var all = await service.GetAllTasksAsync();
        all.Data.Should().HaveCount(1);
        all.Data[0].Title.Should().Be(dto.Title);
    }

    // ✅ تست بروزرسانی تسک
    [Fact]
    public async Task UpdateTaskAsync_Should_Update_Task_Status()
    {
        var context = InMemoryContextFactory.CreateDbContext();
        var service = new TaskService(context, _mapper, _validator);

        // ساخت اولیه تسک
        var createdDto = new TaskDto
        {
            Title = "تسک نمونه",
            Description = "در حال انجام",
            Status = TaskStatus.InProgress,
            Priority = TaskPriority.Medium,
            DueDate = "1404/07/15"
        };
        var created = await service.CreateTaskAsync(createdDto);

        // داده بروزرسانی‌شده
        var updatedDto = new TaskDto
        {
            Title = "تسک نمونه",
            Description = "اکنون تکمیل شده",
            Status = TaskStatus.Completed,
            Priority = TaskPriority.Medium,
            DueDate = "1404/07/15"
        };

        // 🚀  امضای جدید شامل شناسه رکورد + DTO است
        var result = await service.UpdateTaskAsync(created.Data, updatedDto);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeOfType(typeof(int));
        ((int)result.Data).Should().BeGreaterThan(0);

        var fetched = await service.GetTaskByIdAsync(created.Data);
        fetched.Data.Status.Should().Be(TaskStatus.Completed);
        fetched.Data.Description.Should().Be(updatedDto.Description);
    }

    // ✅ تست حذف نرم (Soft Delete)
    [Fact]
    public async Task DeleteTaskAsync_Should_Soft_Delete_Task()
    {
        var context = InMemoryContextFactory.CreateDbContext();
        var service = new TaskService(context, _mapper, _validator);

        var dto = new TaskDto
        {
            Title = "تسک حذف‌شده",
            Description = "برای تست حذف نرم",
            Status = TaskStatus.Pending,
            Priority = TaskPriority.Low,
            DueDate = "1404/07/22"
        };

        var created = await service.CreateTaskAsync(dto);
        var deleted = await service.DeleteTaskAsync(created.Data);

        deleted.Should().NotBeNull();
        deleted.Success.Should().BeTrue();
        ((int)deleted.Data).Should().BeGreaterThan(0);

        var all = await service.GetAllTasksAsync();
        all.Data.Should().OnlyContain(t => !t.IsDeleted);
    }

    // ✅ تست واکشی همه تسک‌ها
    [Fact]
    public async Task GetAllTasksAsync_Should_Return_All_Tasks()
    {
        var context = InMemoryContextFactory.CreateDbContext();
        var service = new TaskService(context, _mapper, _validator);

        await service.CreateTaskAsync(new TaskDto
        {
            Title = "تسک اول",
            Description = "اولین تست",
            Status = TaskStatus.Pending,
            Priority = TaskPriority.High,
            DueDate = "1404/07/23"
        });

        await service.CreateTaskAsync(new TaskDto
        {
            Title = "تسک دوم",
            Description = "تست دوم",
            Status = TaskStatus.Completed,
            Priority = TaskPriority.Medium,
            DueDate = "1404/07/24"
        });

        var result = await service.GetAllTasksAsync();

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    // ✅ تست واکشی بر اساس شناسه
    [Fact]
    public async Task GetTaskByIdAsync_Should_Return_Correct_Task()
    {
        var context = InMemoryContextFactory.CreateDbContext();
        var service = new TaskService(context, _mapper, _validator);

        var dto = new TaskDto
        {
            Title = "تسک سوم",
            Description = "برای تست GetByIdAsync",
            Status = TaskStatus.Pending,
            Priority = TaskPriority.Critical,
            DueDate = "1404/07/25"
        };

        var created = await service.CreateTaskAsync(dto);
        var found = await service.GetTaskByIdAsync(created.Data);

        found.Should().NotBeNull();
        found.Success.Should().BeTrue();
        found.Data.Title.Should().Be(dto.Title);
        found.Data.Status.Should().Be(TaskStatus.Pending);
    }
}
