using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.DTOs;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;
using System.Net;

namespace Rira.Application.Services
{
    // 🧩 سرویس اصلی مدیریت تسک‌ها (TaskService)
    // ===============================================================================
    // این کلاس هسته‌ی منطق کسب‌وکار مربوط به موجودیت «Task» در لایه Application است.
    // مسئولیت این سرویس اجرای عملیات CRUD، اعتبارسنجی، نگاشت داده‌ها و مدیریت وضعیت پاسخ را دارد.
    //
    // 🎯 اهداف طراحی:
    //     ▫ جداسازی منطق کسب‌وکار از Handler و Controller.
    //     ▫ استفاده از ResponseModel<T> برای خروجی‌های استاندارد (موفقیت، خطا، پیام، داده).
    //     ▫ اجرای اعتبارسنجی ورودی‌ها به وسیله‌ی FluentValidation.
    //     ▫ استفاده از AutoMapper جهت نگاشت خودکار DTO ↔ Entity.
    //
    // ⚙️ تزریق وابستگی‌ها:
    //     🟩 IAppDbContext           → دسترسی به DbSet<TaskEntity> و ذخیره‌سازی داده.
    //     🟦 IMapper                 → نگاشت بین DTO و Entity.
    //     🟨 IValidator<TaskDto>     → بررسی قانونی بودن داده‌های ورودی.
    //
    // 🔹 نکته مهم RiRaDocs:
    //     سرویس در معماری Clean Architecture مستقل از Presentation عمل می‌کند؛
    //     هیچ HttpContext یا Controller درون آن وجود ندارد، تنها داده‌های بهینه و پاسخ‌های استاندارد تولید می‌شوند.
    public class TaskService : ITaskService
    {
        // ============================================================
        // 🧩 تعریف وابستگی‌ها (Dependency Injection)
        // ============================================================
        private readonly IAppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<TaskDto> _validator;

        // ------------------------------------------------------------
        // 🧱 سازنده‌ی کلاس
        // ------------------------------------------------------------
        // DbContext، AutoMapper و Validator از طریق DI Container تزریق می‌شوند.
        public TaskService(IAppDbContext dbContext, IMapper mapper, IValidator<TaskDto> validator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _validator = validator;
        }

        // ============================================================
        // 🟩 CreateTaskAsync
        // ------------------------------------------------------------
        // ایجاد تسک جدید با اعتبارسنجی ورودی، نگاشت DTO ↔ Entity و ذخیره در پایگاه داده.
        // ============================================================
        public async Task<ResponseModel<int>> CreateTaskAsync(TaskDto dto)
        {
            // 🧪 مرحله اول: اعتبارسنجی داده‌ها با FluentValidation
            ValidationResult validation = _validator.Validate(dto);
            if (!validation.IsValid)
            {
                string errors = string.Join(" ؛ ", validation.Errors.ConvertAll(e => e.ErrorMessage));
                return ResponseModel<int>.Fail($"داده‌ها معتبر نیستند: {errors}", (int)HttpStatusCode.BadRequest);
            }

            // 🧩 مرحله دوم: نگاشت DTO → Entity
            var entity = _mapper.Map<TaskEntity>(dto);

            // 🗄️ مرحله سوم: ذخیره داده در پایگاه داده از طریق DbContext
            await _dbContext.Tasks.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            // 💬 خروجی نهایی با مدل پاسخ استاندارد
            return ResponseModel<int>.Ok(entity.Id, "✅ تسک با موفقیت ایجاد شد.");
        }

        // ============================================================
        // 🟦 GetAllTasksAsync
        // ------------------------------------------------------------
        // واکشی تمام تسک‌های موجود در پایگاه داده که حذف نرم نشده‌اند.
        // ============================================================
        public async Task<ResponseModel<List<TaskDto>>> GetAllTasksAsync()
        {
            var entities = await _dbContext.Tasks
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            var dtos = _mapper.Map<List<TaskDto>>(entities);

            return ResponseModel<List<TaskDto>>.Ok(dtos, "✅ تمام تسک‌ها با موفقیت واکشی شدند.");
        }

        // ============================================================
        // 🟨 GetTaskByIdAsync
        // ------------------------------------------------------------
        // واکشی اطلاعات تسک مشخص بر اساس شناسه، با بررسی Soft Delete.
        // ============================================================
        public async Task<ResponseModel<TaskDto>> GetTaskByIdAsync(int id)
        {
            var entity = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (entity == null)
                return ResponseModel<TaskDto>.NotFound($"❌ تسک با شناسه {id} یافت نشد.");

            var dto = _mapper.Map<TaskDto>(entity);
            return ResponseModel<TaskDto>.Ok(dto, "✅ تسک با موفقیت یافت شد.");
        }

        // ============================================================
        // 🟧 UpdateTaskAsync
        // ------------------------------------------------------------
        // بروزرسانی داده‌های تسک موجود با توجه به DTO جدید و اعتبارسنجی آن.
        // ============================================================
        public async Task<ResponseModel<int>> UpdateTaskAsync(int id, TaskDto dto)
        {
            var entity = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (entity == null)
                return ResponseModel<int>.NotFound($"❌ تسک با شناسه {id} یافت نشد.");

            // 🧪 اعتبارسنجی ورودی قبل از اعمال تغییرات
            ValidationResult validation = _validator.Validate(dto);
            if (!validation.IsValid)
            {
                string errors = string.Join(" ؛ ", validation.Errors.ConvertAll(e => e.ErrorMessage));
                return ResponseModel<int>.Fail($"داده‌های وارد شده معتبر نیستند: {errors}", (int)HttpStatusCode.BadRequest);
            }

            // 🧩 نگاشت DTO → Entity و بروزرسانی زمان آخرین تغییر
            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.Now.ToString("yyyy/MM/dd");

            _dbContext.Tasks.Update(entity);
            await _dbContext.SaveChangesAsync();

            return ResponseModel<int>.Ok(entity.Id, "✅ تسک با موفقیت بروزرسانی شد.");
        }

        // ============================================================
        // 🟥 DeleteTaskAsync
        // ------------------------------------------------------------
        // حذف نرم تسک (Soft Delete) — تغییر وضعیت بدون حذف فیزیکی از پایگاه داده.
        // ============================================================
        public async Task<ResponseModel<int>> DeleteTaskAsync(int id)
        {
            var entity = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (entity == null)
                return ResponseModel<int>.NotFound($"❌ تسک با شناسه {id} یافت نشد.");

            entity.IsDeleted = true;
            _dbContext.Tasks.Update(entity);
            await _dbContext.SaveChangesAsync();

            return ResponseModel<int>.Ok(entity.Id, "🗑️ تسک با موفقیت حذف شد (Soft Delete).");
        }
    }

    // ===========================================================================================
    // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
    // -------------------------------------------------------------------------------------------
    // 🔹 مفهوم: TaskService قلب منطق Application برای مدیریت موجودیت‌های Task است.
    // 🔹 اهداف کلیدی:
    //     ▫ هماهنگ‌سازی عملیات CRUD با EF Core + AutoMapper + FluentValidation.
    //     ▫ جلوگیری از تکرار کد در Handlerها با متمرکزسازی منطق در سرویس.
    //     ▫ بازگرداندن پاسخ استاندارد از نوع ResponseModel<T> همراه کد وضعیت HTTP.
    //
    // 🔹 جریان CQRS ترکیبی:
    //     ▫ Command → Handler → فراخوانی سرویس → ذخیره در DbContext.
    //     ▫ Query → Handler → سرویس → برگشت ResponseModel<T>.
    //
    // 🔹 اصول معماری:
    //     ▫ Clean Architecture — Application مستقل از UI.
    //     ▫ Separation of Concerns — هر مسئولیت در جای خود.
    //     ▫ Soft Delete Pattern برای حفظ داده‌ها و گزارش تغییرات.
    //     ▫ Async Programming جهت بهینه‌سازی عملکرد I/O.
    //
    // 🔹 تگ انتشار RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===========================================================================================
}
