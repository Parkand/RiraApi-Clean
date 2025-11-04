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
// در معماری Clean Architecture این فایل نقش "root composition" دارد:
// یعنی محل اتصال و پیکربندی همه‌ی وابستگی‌ها، سرویس‌ها، میان‌افزارها و ماژول‌های کلیدی.
//

var builder = WebApplication.CreateBuilder(args);

//
// ================================================
// 🔹 بخش اول: ثبت سرویس‌ها و تنظیمات Dependency Injection (DI)
// ================================================
//
// DI در ASP.NET Core یک مکانیزم تزریق وابستگی است که به ما اجازه می‌دهد
// اشیاء مورد نیاز کلاس‌ها را بدون ساخت صریح، از بیرون تزریق کنیم.
// این کار باعث می‌شود کد تست‌پذیرتر، منعطف‌تر و قابل نگهداری‌تر باشد.
//

// ✅ ثبت DbContext و اینترفیس دامنه برای EF Core 8
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // "DefaultConnection" از appsettings.json خوانده می‌شود.
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// در اینجا IAppDbContext برای تست‌ها و جداسازی لایه Persistence استفاده می‌شود.
builder.Services.AddScoped<IAppDbContext, AppDbContext>();

// ✅ ثبت سرویس‌های اصلی اپلیکیشن (لایه Application)
// این سرویس‌ها شامل منطق تجاری مرتبط با Entities هستند.
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ITaskService, TaskService>();

//
// ✅ AutoMapper — ابزار نگاشت بین DTOها و Entityها.
//
builder.Services.AddAutoMapper(typeof(EmployeeProfile).Assembly);

// AutoMapper به‌صورت خودکار همه‌ی MappingProfileهای تعریف‌شده در Assembly Application را اسکن می‌کند.
//

//
// ✅ FluentValidation — ولیدیشن داده‌ها به‌صورت جدا از مدل‌ها
//
// Validators در Assembly Application اسکن و اتوماتیک ثبت می‌شوند.
// این روش بهترین شیوه برای حفظ تمیزی کد و جدایی وظایف است.
//
builder.Services.AddValidatorsFromAssembly(typeof(EmployeeProfile).Assembly);

//
// ✅ MediatR — ابزار اجرای Command/Query Handlerها در سبک CQRS
//
// MediatR واسطه‌ی بین Controller (درخواست ورودی) و Handler (منطق تجاری) است.
// بدینصورت کنترلر فقط درخواست را ارسال می‌کند و Handler نتیجه را برمی‌گرداند.
//
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(EmployeeProfile).Assembly);
});

//
// ✅ ثبت کنترلرهای API و تنظیمات پیشرفته‌ی JSON برای سریال‌سازی
//
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        // جلوگیری از حلقه‌های تودرتوی JSON در EF Core (مثلاً Employee → Department → Employee)
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        // نمایش Enumها به‌صورت رشته به جای عدد (مثلاً "Active" به جای 1)
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

        // حفظ PascalCase برای نام ویژگی‌ها به‌جای camelCase
        opt.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

//
// ✅ اضافه کردن Swagger برای مستندسازی و تست راحت در UI مرورگر
//
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
// 🔹 بخش دوم: ساخت Environment و تنظیم Middlewareها
// ================================================
//
// پس از ثبت سرویس‌ها، متد Build شیء WebApplication را ایجاد می‌کند.
// حالا Middlewareها و مسیرهای کنترلر را فعال می‌کنیم.
//

var app = builder.Build();

//
// ✅ فعال‌سازی Swagger فقط در حالت توسعه (Development)
// یعنی در محیط Production غیرفعال می‌شود برای امنیت بیشتر.
//
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rira.Api v1");
        // مسیر مدارک مرورگر: https://localhost:5001/docs
        c.RoutePrefix = "docs";
    });
}

// ✅ فعال‌سازی HTTPS برای ارتباط امن بین کلاینت و سرور
app.UseHttpsRedirection();

// ✅ Authorization برای کنترل سطح دسترسی‌های کاربر
app.UseAuthorization();

//
// ✅ افزودن مسیرهای کنترلرها به Pipeline — تمام کنترلرهایی که [ApiController] دارند در اینجا فعال می‌شوند.
//
app.MapControllers();

//
// ================================================
// 🔹 مرحلهٔ نهایی اجرای WebApplication
// ================================================
//
app.Run();

//
// ✅ جمع‌بندی آموزشی:
//
// ✳️ این فایل ریشه‌ی پیکربندی پروژه است و فلسفه‌ی Clean Architecture را اجرا می‌کند.
// ✳️ لایه‌ی Application سرویس‌ها، ولیدیشن و CQRS را مدیریت می‌کند.
// ✳️ لایه‌ی Persistence ارتباط با دیتابیس را از طریق EF Core 8 فراهم می‌سازد.
// ✳️ کنترلرها فقط نقش رابط API را دارند بدل به محل اجرای منطق تجاری نمی‌شوند.
//
