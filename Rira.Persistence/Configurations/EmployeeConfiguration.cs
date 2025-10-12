using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rira.Domain.Entities;
using System;

namespace Rira.Persistence.Configurations
{
    /// <summary>
    /// ⚙️ کلاس پیکربندی EF Core برای موجودیت EmployeeEntity
    /// ----------------------------------------------------
    /// وظیفه:
    /// - تنظیم خصوصیات جدول Employees در دیتابیس.
    /// - تعریف محدودیت‌ها، تبدیل Enumها، ایندکس‌ها، و داده‌های اولیه (Seed Data).
    /// مطابق استاندارد معماری Clean در پروژه Rira.
    /// </summary>
    public class EmployeeConfiguration : IEntityTypeConfiguration<EmployeeEntity>
    {
        public void Configure(EntityTypeBuilder<EmployeeEntity> builder)
        {
            // 📌 نام جدول در دیتابیس
            builder.ToTable("Employees");

            // 🧩 کلید اصلی (Id)
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd();

            // =========================================================
            // 🧩 تنظیمات رشته‌ها و طول
            // =========================================================
            builder.Property(e => e.FirstName)
                   .IsRequired()
                   .HasMaxLength(60);

            builder.Property(e => e.LastName)
                   .IsRequired()
                   .HasMaxLength(60);

            builder.Property(e => e.Position)
                   .IsRequired()
                   .HasMaxLength(80);

            builder.Property(e => e.Email)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(e => e.MobileNumber)
                   .IsRequired()
                   .HasMaxLength(11);

            builder.Property(e => e.Description)
                   .HasMaxLength(500);

            builder.Property(e => e.BirthDatePersian)
                   .HasMaxLength(10);

            builder.Property(e => e.FieldOfStudy)
                   .HasMaxLength(100);

            // =========================================================
            // 🔢 تبدیل Enumها به نوع عددی (int) در دیتابیس
            // =========================================================
            builder.Property(e => e.Gender)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(e => e.EducationLevel)
                   .HasConversion<int>()
                   .IsRequired();

            // =========================================================
            // 🧮 تنظیم پیش‌فرض‌ها
            // =========================================================
            builder.Property(e => e.IsActive).HasDefaultValue(true);
            builder.Property(e => e.HireDate)
                   .HasDefaultValueSql("GETDATE()"); // پیش‌فرض زمان فعلی SQL

            // =========================================================
            // 🗄️ ایندکس‌ها برای بهینه‌سازی کوئری‌ها
            // =========================================================
            builder.HasIndex(e => e.Email).IsUnique();
            builder.HasIndex(e => e.MobileNumber).IsUnique();
            builder.HasIndex(e => e.Position);

            // =========================================================
            // 🌱 داده‌های اولیه (Seed Data)
            // =========================================================
            builder.HasData(
                //
                // 🧑‍💻 مثال ۱: توسعه‌دهنده اصلی پروژه ریرا (سروش)
                new EmployeeEntity
                {
                    Id = 1,
                    FirstName = "سروش",
                    LastName = "مغربی",
                    Gender = EmployeeEntity.GenderType.Male,
                    MobileNumber = "09120000000",
                    Email = "parkand@github.com",
                    BirthDatePersian = "1370/05/21",
                    EducationLevel = EmployeeEntity.EducationLevelType.Doctorate,
                    FieldOfStudy = "مهندسی نرم‌افزار",
                    Position = "Lead Developer",
                    HireDate = DateTime.Now,
                    IsActive = true,
                    Description = "توسعه‌دهنده اصلی پروژه Rira.Api"
                },

                //
                // 👨‍🏫 مثال ۲: مدیر منابع انسانی آزمایشی
                new EmployeeEntity
                {
                    Id = 2,
                    FirstName = "علی",
                    LastName = "کاظمی",
                    Gender = EmployeeEntity.GenderType.Male,
                    MobileNumber = "09121234567",
                    Email = "ali.kazemi@rira.local",
                    BirthDatePersian = "1368/11/12",
                    EducationLevel = EmployeeEntity.EducationLevelType.Master,
                    FieldOfStudy = "مدیریت منابع انسانی",
                    Position = "HR Manager",
                    HireDate = DateTime.Now,
                    IsActive = true,
                    Description = "نمونه تستی برای بررسی عملکرد Configuration"
                }
            );

            // =========================================================
            // 🏁 پایان تنظیمات جدول EmployeeEntity
            // =========================================================
        }
    }
}
