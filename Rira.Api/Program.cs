using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rira.Application.Interfaces;
using Rira.Application.MappingProfiles;
using Rira.Application.Services;
using Rira.Persistence.Data;
using System.Text.Json.Serialization;

//
// 🏫 توضیح کلی:
//
// فایل Program.cs نقطه‌ی ورودی اصلی پروژه‌ی Rira.Api محسوب می‌شود.
// در معماری Clean Architecture این فایل نقش "Root Composition" را دارد:
// یعنی محل اتصال و پیکربندی تمام وابستگی‌ها، سرویس‌ها، میان‌افزارها‌ و ماژول‌های کلیدی.
//

var builder = WebApplication.CreateBuilder(args);

//
// ================================================
// 🔹 بخش اول: ثبت سرویس‌ها و تنظیمات Dependency Injection (DI)
// ================================================
//

// ✅ DbContext و اتصال به SQL Server از appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// جداسازی لایه Persistence با IAppDbContext
builder.Services.AddScoped<IAppDbContext, AppDbContext>();

// ✅ ثبت سرویس‌های اصلی اپلیکیشن
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// ✅ AutoMapper — نگاشت DTO ↔ Entity
builder.Services.AddAutoMapper(cfg => { }, typeof(EmployeeProfile).Assembly);

// AutoMapper به‌صورت خودکار همه‌ی MappingProfileهای تعریف‌شده در Assembly Application را اسکن می‌کند.

// ✅ FluentValidation — ولیدیشن داده‌ها
builder.Services.AddValidatorsFromAssembly(typeof(EmployeeProfile).Assembly);

// ✅ MediatR — هماهنگ‌کننده Command و Query در CQRS
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(EmployeeProfile).Assembly);
});

// ✅ پیکربندی کنترلرها و Serialization JSON
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        opt.JsonSerializerOptions.PropertyNamingPolicy = null; // حفظ PascalCase
    });

// ✅ Swagger برای مستندسازی و تست سریع API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Rira.Api — Clean Architecture Finalized",
        Version = "v1"
    });
});

//
// ================================================
// 🔹 بخش دوم: ساخت Pipeline و Middlewareها
// ================================================
//

var app = builder.Build();

//
// ✅ فعال‌سازی Swagger در محیط توسعه (Development)
//
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rira.Api v1");
        // مسیر رابط مرورگر Swagger:
        // https://localhost:7044/docs
        c.RoutePrefix = "docs";
    });
}

// ✅ فعال‌سازی HTTPS برای ارتباط امن
app.UseHttpsRedirection();

// ✅ احراز هویت و مجوز (در صورت نیاز در آینده)
app.UseAuthorization();

// ✅ نگاشت کنترلرهای API به Pipeline
app.MapControllers();

//
// ================================================
// 🔹 مرحلهٔ نهایی اجرای WebApplication
// ================================================
app.Run();

//
// ✅ جمع‌بندی آموزشی:
//
// ✳️ این فایل ریشه‌ی پیکربندی پروژه است و فلسفه‌ی Clean Architecture را پیاده می‌کند.
// ✳️ لایه‌ی Application شامل سرویس‌ها، ولیدیشن و CQRS است.
// ✳️ لایه‌ی Persistence وظیفه‌ی اتصال به پایگاه داده را دارد.
// ✳️ کنترلرها تنها نقش ورودی/خروجی (API Interface) را ایفا می‌کنند.
//
