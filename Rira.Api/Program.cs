// ----------------- فضاهای نام ضروری (Using) -----------------
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Rira.Application.Interfaces;
using Rira.Application.MappingProfiles;
using Rira.Application.Services;
using Rira.Application.Validators;
using Rira.Persistence.Data;


var builder = WebApplication.CreateBuilder(args);

// ----------------- ثبت سرویس‌ها در کانتینر DI -----------------

// ۱. ثبت کنترلرها و ابزارهای استاندارد API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 🟦 SwaggerGen به‌صورت کامل طبق استاندارد OpenAPI v3.0
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Rira API",
        Version = "v1",
        Description = "مستندات API پروژه ریرا با معماری تمیز",
    });

    // فعال‌سازی Annotationها (برای توضیحات در سطح Operation)
    options.EnableAnnotations();
});

// ۲. پیکربندی AutoMapper (نسخه پایدار طبق درخواست شما)
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<TaskProfile>();
});

// ۳. ثبت FluentValidation (روش صحیح)
builder.Services.AddValidatorsFromAssemblyContaining<TaskDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();
// ❌ AddFluentValidationClientsideAdapters برای API لازم نیست

// ۴. اتصال به دیتابیس
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ۵. تزریق وابستگی‌های لایه‌ها (مطابق با معماری تمیز)
builder.Services.AddScoped<IAppDbContext, AppDbContext>();
builder.Services.AddScoped<ITaskService, TaskService>();

// ----------------- ساخت و پیکربندی Pipeline برنامه -----------------
var app = builder.Build();

// ✅ فعال‌سازی Swagger بدون شرط محیط (هم در توسعه و هم در اجرا داخلی)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rira API v1");
    c.RoutePrefix = string.Empty; // نمایش Swagger در ریشه (/)
});

app.UseHttpsRedirection();
app.UseCors(policy => policy.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod());
app.UseAuthorization();
app.MapControllers();

// ----------------- انتهای کلاس تسک -----------------
app.Run();
