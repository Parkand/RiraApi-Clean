using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rira.Application.Interfaces;
using Rira.Application.MappingProfiles;
using Rira.Application.Services;
using Rira.Persistence.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ================================================
// 🔹 ثبت سرویس‌ها و تنظیمات Dependency Injection
// ================================================

// ✅ DbContext و Interface مربوطه
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IAppDbContext, AppDbContext>();

// ✅ ثبت سرویس‌های حوزه Application
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// ✅ AutoMapper — اسکن کل اسمبلی Application
builder.Services.AddAutoMapper(typeof(EmployeeProfile).Assembly);

// ✅ Validators — افزودن تمام Validatorها از Assembly Application
builder.Services.AddValidatorsFromAssembly(typeof(EmployeeProfile).Assembly);

// ✅ MediatR — ثبت همه‌ی Command و Query Handlerها در Application
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(EmployeeProfile).Assembly);
});

// ✅ کنترلرها و تنظیمات JSON
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        // جلوگیری از حلقه‌های سریال‌سازی
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // نمایش Enumها به صورت رشته
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        // حفظ PascalCase برای ویژگی‌ها
        opt.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// ✅ Swagger برای مستندسازی و تست از طریق UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Rira.Api — Clean Architecture Finalized",
        Version = "v1"
    });
});

// ================================================
// 🔹 ساخت Environment و تنظیم Middlewareها
// ================================================

var app = builder.Build();

// ✅ فعال‌سازی Swagger فقط در حالت توسعه
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rira.Api v1");
        c.RoutePrefix = "docs"; // مثال: https://localhost:5001/docs
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

// ✅ ثبت کنترلرها
app.MapControllers();

// ================================================
// 🔹 اجرای نهایی
// ================================================

app.Run();
