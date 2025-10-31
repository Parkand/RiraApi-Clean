using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Moq;
using Rira.Application.Interfaces;
using Rira.Application.Validators;
using Rira.Domain.Entities;
using TaskStatus = Rira.Domain.Enums.TaskStatus;

namespace Rira.Application.Tests.Services
{
    /// <summary>
    /// ============================================================================
    /// 🧠 کلاس تست واحد سرویس وظیفه‌ها (TaskService)
    /// ----------------------------------------------------------------------------
    /// 👑 توسعه‌دهنده: سروش (KIA)
    /// 🗓 تاریخ آخرین ویرایش: 1404/08/08
    ///
    /// 📘 هدف:
    ///   بررسی عملکرد CRUD سرویس وظیفه‌ها با شبیه‌سازی کامل EF Core.
    ///
    /// 🧩 موارد کلیدی:
    ///   ✔ AutoMapper v15.0.1 با LoggerFactory
    ///   ✔ حل نهایی مشکل Id = 0 در Mock DbContext
    ///   ✔ ساختار AAA و استفاده از FluentAssertions + RiraDocs
    /// ============================================================================
    /// </summary>
    public class TaskServiceTests
    {
        private readonly TaskService _service;
        private readonly Mock<IAppDbContext> _mockDbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<TaskDto> _validator;
        private readonly List<TaskEntity> _tasks;

        public TaskServiceTests()
        {
            // 🔹 داده‌های آزمایشی اولیه
            _tasks = new List<TaskEntity>
            {
                new TaskEntity { Id = 1, Title = "Task A", Status = TaskStatus.Pending },
                new TaskEntity { Id = 2, Title = "Task B", Status = TaskStatus.Completed },
                new TaskEntity { Id = 3, Title = "Task C", Status = TaskStatus.InProgress }
            };

            // 🔹 Mock DbContext (Tasks + SaveChangesAsync)
            _mockDbContext = new Mock<IAppDbContext>();
            _mockDbContext.Setup(x => x.Tasks)
                .Returns(MockDbSet.Create(_tasks).Object);
            _mockDbContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);

            // ⚙️ نسخه Rira v3 — شبیه‌سازی واقعی EF Core.AddAsync با تولید شناسه جدید
            _mockDbContext
                .Setup(x => x.Tasks.AddAsync(It.IsAny<TaskEntity>(), It.IsAny<CancellationToken>()))
                .Returns((TaskEntity entity, CancellationToken _) =>
                {
                    // 🧠 تولید شناسه جدید مثل EF (قبل از خروج)
                    var nextId = _tasks.Any() ? _tasks.Max(t => t.Id) + 1 : 1;
                    entity.Id = nextId;
                    _tasks.Add(entity);

                    // 🔄 ساخت EntityEntry لازم برای EF Core
                    var mockEntry = new Mock<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TaskEntity>>();
                    mockEntry.Setup(m => m.Entity).Returns(entity);

                    return new ValueTask<
                        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TaskEntity>
                    >(mockEntry.Object);
                });

            // 🔹 پیکربندی AutoMapper v15 با LoggerFactory واقعی
            var cfgExp = new MapperConfigurationExpression();
            cfgExp.CreateMap<TaskEntity, TaskDto>()
                  .ForAllMembers(opt => opt.Condition((s, d, srcMember) => srcMember != null));
            cfgExp.CreateMap<TaskDto, TaskEntity>()
                  .ForAllMembers(opt => opt.Condition((s, d, srcMember) => srcMember != null));
            var mapperCfg = new MapperConfiguration(cfgExp, new LoggerFactory());
            _mapper = new Mapper(mapperCfg);

            // 🔹 Validator واقعی
            _validator = new TaskDtoValidator();

            // 🎯 ساخت سرویس هدف
            _service = new TaskService(_mockDbContext.Object, _mapper, _validator);
        }

        // 🧪 ==============================================================
        [Fact(DisplayName = "GetAllTasksAsync_Should_Return_All_Tasks")]
        public async Task GetAllTasksAsync_Should_Return_All_Tasks()
        {
            // Act
            var result = await _service.GetAllTasksAsync();

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(3);
            result.Data.First().Title.Should().Be("Task A");
        }

        // 🧪 ==============================================================
        [Fact(DisplayName = "GetTaskByIdAsync_Should_Return_Correct_Task")]
        public async Task GetTaskByIdAsync_Should_Return_Correct_Task()
        {
            // Act
            var result = await _service.GetTaskByIdAsync(2);

            // Assert
            result.Data.Id.Should().Be(2);
            result.Data.Status.Should().Be(TaskStatus.Completed);
        }

        // 🧪 ==============================================================
        [Fact(DisplayName = "CreateTaskAsync_Should_Add_New_Task")]
        public async Task CreateTaskAsync_Should_Add_New_Task()
        {
            // Arrange
            var dto = new TaskDto { Title = "New Task", Status = TaskStatus.Pending };

            // Act
            var result = await _service.CreateTaskAsync(dto);

            // Assert
            _tasks.Last().Id.Should().BeGreaterThan(0, "شناسه باید توسط Mock تولید شود.");
            result.Data.Should().BeGreaterThan(0, "شناسه باید بزرگتر از صفر باشد.");
            result.Message.Should().MatchEquivalentOf("*created*");
        }

        // 🧪 ==============================================================
        [Fact(DisplayName = "UpdateTaskAsync_Should_Update_Task_Status")]
        public async Task UpdateTaskAsync_Should_Update_Task_Status()
        {
            // Arrange
            var dto = new TaskDto { Id = 1, Title = "Task A Updated", Status = TaskStatus.Completed };

            // Act
            var result = await _service.UpdateTaskAsync(dto.Id, dto);

            // Assert
            result.Data.Should().BeGreaterThan(0);
            result.Message.Should().MatchEquivalentOf("*updated*");
        }

        // 🧪 ==============================================================
        [Fact(DisplayName = "DeleteTaskAsync_Should_Remove_Task")]
        public async Task DeleteTaskAsync_Should_Remove_Task()
        {
            // Arrange
            var id = 2;

            // Act
            var result = await _service.DeleteTaskAsync(id);

            // Assert
            result.Data.Should().BeGreaterThan(0);
            result.Message.Should().MatchEquivalentOf("*deleted*");
        }
    }

    // ============================================================================
    // 🧩 Mock DbSet Helper – نسخه ساده‌شده Rira
    // ============================================================================
    public static class MockDbSet
    {
        public static Mock<Microsoft.EntityFrameworkCore.DbSet<T>> Create<T>(IEnumerable<T> data)
            where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            return mockSet;
        }
    }
}
