// ===========================================================
// 📘 RiRaDocs Teaching Edition (Farsi Inline)
// File: EmployeeServiceIntegrationTests.cs
// Layer: Integration Tests
// Context: Clean Architecture — Application ⇆ Persistence
// هدف: تست ادغام واقعی سرویس EmployeeService با DI، AutoMapper، FluentValidation و EF Core 8
// انتشار: RiraDocs‑v2025.11.5‑Stable‑Final‑Fixed
// ===========================================================

using System;
using System.Threading.Tasks;
using FluentValidation;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting; // ✅ لازم برای [TestClass], [TestMethod], [ExpectedException]
using Rira.Application.DTOs;
using Rira.Application.Features.Employees.Commands.CreateEmployee;
using Rira.Application.Features.Employees.Commands.UpdateEmployee;
using Rira.Application.Interfaces;
using Rira.Application.MappingProfiles;
using Rira.Application.Services;
using Rira.Domain.Entities;
using Rira.Persistence.Data;
using Rira.Application.Common.Exceptions; // ✅ برای دسترسی به NotFoundException

namespace Rira.IntegrationTests
{
    /// <summary>
    /// 🧩 کلاس تست ادغام سرویس EmployeeService
    /// بررسی رفتار واقعی سرویس در محیط InMemory + DI + AutoMapper + FluentValidation
    /// </summary>
    [TestClass]
    public class EmployeeServiceIntegrationTests
    {
        // ⚙️ Provider اصلی DI
        private ServiceProvider _provider = null!;
        // 🎯 سرویس اصلی مورد تست
        private IEmployeeService _service = null!;

        // ===========================================================
        // 🔧 Setup — پیکربندی کانتینر وابستگی‌ها و سرویس‌ها
        // ===========================================================
        [TestInitialize]
        public void Setup()
        {
            var services = new ServiceCollection();

            // 🟢 ثبت DbContext با بانک داده InMemory
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseInMemoryDatabase(databaseName: "RiraEmployeeIntegrationDb"));

            // 🟢 اتصال Application ↔ Persistence
            services.AddScoped<IAppDbContext>(sp =>
                sp.GetRequiredService<AppDbContext>());

            // 🟢 پیکربندی AutoMapper
            services.AddAutoMapper(cfg => cfg.AddProfile<EmployeeProfile>());

            // 🟢 Validators واقعی
            services.AddValidatorsFromAssemblyContaining<EmployeeCreateCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<EmployeeUpdateValidator>();

            // 🟢 سرویس اصلی
            services.AddScoped<IEmployeeService, EmployeeService>();

            // 💠 ساخت Provider و Resolve نهایی
            _provider = services.BuildServiceProvider();
            _service = _provider.GetRequiredService<IEmployeeService>();
        }

        // ===========================================================
        // ✅ CreateAsync Test
        // ===========================================================
        [TestMethod]
        public async Task CreateAsync_Should_Add_New_Employee()
        {
            var dto = new EmployeeDTO
            {
                FirstName = "سروش",
                LastName = "یوسفی",
                Gender = EmployeeDTO.GenderType.Male,
                EducationLevel = EmployeeDTO.EducationLevelType.Master,
                FieldOfStudy = "Software Engineering",
                MobileNumber = "09120000000",
                BirthDatePersian = "1375/01/01",
                Position = "Lead Developer",
                Email = "soroush.integration@example.com",
                HireDate = DateTime.UtcNow,
                IsActive = true,
                Description = "ایجاد شده در تست ادغام"
            };

            var result = await _service.CreateAsync(dto);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Email.Should().Be("soroush.integration@example.com");
        }

        // ===========================================================
        // ✅ UpdateAsync Test
        // ===========================================================
        [TestMethod]
        public async Task UpdateAsync_Should_Modify_Employee_Data()
        {
            var ctx = _provider.GetRequiredService<AppDbContext>();
            var entity = new EmployeeEntity
            {
                FirstName = "زهرا",
                LastName = "حسینی",
                Email = "zahra@example.com",
                Gender = EmployeeEntity.GenderType.Female,
                HireDate = DateTime.UtcNow,
                IsActive = true
            };
            ctx.Employees.Add(entity);
            await ctx.SaveChangesAsync();

            var dto = new EmployeeDTO
            {
                Id = entity.Id,
                FirstName = "زهرا",
                LastName = "کریمی",
                Gender = EmployeeDTO.GenderType.Female,
                EducationLevel = EmployeeDTO.EducationLevelType.Bachelor,
                Email = "zahra.karimi@example.com",
                Position = "HR Specialist",
                HireDate = entity.HireDate,
                IsActive = true
            };

            var result = await _service.UpdateAsync(dto);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();

            var updated = await ctx.Employees.FindAsync(entity.Id);
            updated!.LastName.Should().Be("کریمی");
            updated.Email.Should().Be("zahra.karimi@example.com");
        }

        // ===========================================================
        // ✅ DeleteAsync Test
        // ===========================================================
        [TestMethod]
        public async Task DeleteAsync_Should_Remove_Employee()
        {
            var ctx = _provider.GetRequiredService<AppDbContext>();
            var entity = new EmployeeEntity
            {
                FirstName = "علی",
                LastName = "موسوی",
                Email = "ali.delete@example.com",
                HireDate = DateTime.UtcNow,
                IsActive = true
            };
            ctx.Employees.Add(entity);
            await ctx.SaveChangesAsync();

            var result = await _service.DeleteAsync(entity.Id);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();

            var check = await ctx.Employees.FindAsync(entity.Id);
            check.Should().BeNull();
        }

        // ===========================================================
        // ✅ NotFoundException Test
        // ===========================================================
        [TestMethod]
        public async Task GetByIdAsync_Should_Throw_NotFound_For_InvalidId()
        {
            // Act
            Func<Task> act = async () => await _service.GetByIdAsync(Guid.NewGuid());

            // Assert — با FluentAssertions یا Assert کلاس MSTest
            await act.Should().ThrowAsync<Rira.Application.Common.Exceptions.NotFoundException>();
        }


        // ===========================================================
        // 🧹 Cleanup
        // ===========================================================
        [TestCleanup]
        public void Cleanup() => _provider.Dispose();
    }
}

// ===========================================================
// 📘 RiRaDocs Summary
// -----------------------------------------------------------
// ▫ بررسی رفتار واقعی سرویس زیر بار InMemory و DI
// ▫ صحت AutoMapper و FluentValidation
// ▫ عملکرد ResponseModel استاندارد پروژه (Ok / Fail)
// ▫ آموزش AAA Pattern پنج‌مرحله‌ای RiRaDocs
// ▫ تگ انتشار: RiraDocs‑v2025.11.5‑Stable‑Final‑Fixed
// ===========================================================
