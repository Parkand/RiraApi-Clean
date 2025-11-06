using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rira.Domain.Entities;
using System;

namespace Rira.Persistence.Configurations
{
    // ⚙️ کلاس پیکربندی EF Core برای موجودیت EmployeeEntity
    // ===============================================================================
    // این کلاس مسئول تعیین ویژگی‌های جدول "Employees" در دیتابیس است؛
    // شامل تعریف کلیدها، نوع داده‌ها، ایندکس‌ها، مقادیر پیش‌فرض و داده‌های نمونه اولیه.
    //
    // 🎯 اهداف RiRaDocs:
    //     ▫ سهولت تست‌پذیری (Testability) و انزوا از EF.
    //     ▫ اطمینان از مطابقت ساختار داده‌ها با قوانین دامنه در کلاس EmployeeEntity.
    //     ▫ ارائه درک آموزشی از نقش Configurations در Clean Architecture.
    //
    // 💡 چکیده آموزشی:
    //     در معماری تمیز ریرا، این کلاس در لایه‌ی Persistence قرار دارد؛
    //     یعنی وابستگی از دامنه به دیتابیس اینجا مدیریت می‌شود نه در Entity.
    public class EmployeeConfiguration : IEntityTypeConfiguration<EmployeeEntity>
    {
        public void Configure(EntityTypeBuilder<EmployeeEntity> builder)
        {
            // -----------------------------------------------------------------------
            // 📌 نام جدول در دیتابیس
            // -----------------------------------------------------------------------
            builder.ToTable("Employees");

            // -----------------------------------------------------------------------
            // 🧩 کلید اصلی (Primary Key)
            // -----------------------------------------------------------------------
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd();

            // -----------------------------------------------------------------------
            // 🧩 تنظیمات ویژگی‌های رشته‌ای با محدودیت طول
            // -----------------------------------------------------------------------
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

            // -----------------------------------------------------------------------
            // 🔢 تبدیل Enumها به نوع عددی در دیتابیس (HasConversion<int>())
            // -----------------------------------------------------------------------
            // در دامنه از Enumهای GenderType و EducationLevelType استفاده می‌شود،
            // ولی در دیتابیس این مقادیر به صورت عددی ذخیره می‌گردند.
            builder.Property(e => e.Gender)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(e => e.EducationLevel)
                   .HasConversion<int>()
                   .IsRequired();

            // -----------------------------------------------------------------------
            // ⚙️ تنظیم مقادیر پیش‌فرض برای برخی ویژگی‌ها
            // -----------------------------------------------------------------------
            builder.Property(e => e.IsActive)
                   .HasDefaultValue(true);

            builder.Property(e => e.HireDate)
                   .HasDefaultValueSql("GETDATE()"); // زمان فعلی SQL هنگام درج

            // -----------------------------------------------------------------------
            // 🗄️ تعریف ایندکس‌ها برای بهینه‌سازی کوئری‌ها
            // -----------------------------------------------------------------------
            builder.HasIndex(e => e.Email).IsUnique();          // جلوگیری از ایمیل تکراری
            builder.HasIndex(e => e.MobileNumber).IsUnique();   // جلوگیری از تلفن تکراری
            builder.HasIndex(e => e.Position);                  // برای فیلتر سریع موقعیت‌ها

            // -----------------------------------------------------------------------
            // 🌱 داده‌های اولیه (Seed Data)
            // -----------------------------------------------------------------------
            // این داده‌ها در زمان اجرای Migration اضافه می‌شوند.
            // هدف، ایجاد کاربر نمونه برای تست اولیه لایه‌های Application و API است.
            builder.HasData(
                // 🧑‍💻 مثال ۱: توسعه‌دهنده اصلی پروژه ریرا
                new EmployeeEntity
                {
                    Id = new Guid(),
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

                // 👨‍🏫 مثال ۲: مدیر منابع انسانی نمونه
                new EmployeeEntity
                {
                    Id = new Guid(),
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

            // -----------------------------------------------------------------------
            // 🏁 پایان پیکربندی جدول EmployeeEntity
            // -----------------------------------------------------------------------
        }
    }

    // ===============================================================================
    // 📘 خلاصه آموزشی RiRaDocs Teaching Edition
    // ------------------------------------------------------------------------------
    // 🔹 نقش این کلاس در معماری تمیز:
    //     ▫ قرارگیری در لایه‌ی Persistence برای پیاده‌سازی پایگاه داده.
    //     ▫ جداسازی منطق پیکربندی از تعریف اصلی دامنه (Domain).
    //
    // 🔹 وابستگی‌ها و ارتباطات:
    //     ▫ EmployeeEntity → تعریف مدل دامنه.
    //     ▫ IAppDbContext → شامل DbSet<EmployeeEntity>.
    //
    // 🔹 اصول طراحی رعایت‌شده:
    //     ▫ Separation of Concerns — تفکیک کامل منطق پیکربندی از لایه‌ی دامین.
    //     ▫ Ensuring Integrity — تضمین یکتایی ایمیل/شماره موبایل.
    //     ▫ Domain Consistency — انطباق Enumها با نوع داده‌ی عددی پایگاه داده.
    //
    // 🔹 تگ انتشار RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===============================================================================
}
