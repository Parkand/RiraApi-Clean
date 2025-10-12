using AutoMapper;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.DTOs;
using Rira.Application.Interfaces; // ⬅️ محل تعریف IAppDbContext
using Rira.Application.Validators;
using Rira.Domain.Entities;

namespace Rira.Application.Services
{
    /// <summary>
    /// ===========================================================================
    /// ✅ سرویس اصلی مدیریت تسک‌ها (TaskService)
    /// ---------------------------------------------------------------------------
    /// این کلاس در لایه‌ی Application قرار دارد و برای تعامل با داده‌ها از
    /// اینترفیس IAppDbContext استفاده می‌کند.
    ///
    /// مزیت استفاده از IAppDbContext بجای AppDbContext:
    ///   - جداسازی کامل از لایه Persistence
    ///   - امکان تست‌پذیری بالا در تست‌های واحد و Integration
    ///   - پیروی از اصل Dependency Inversion
    /// ---------------------------------------------------------------------------
    /// تمام خروجی‌ها در قالب ResponseModel<T> برگردانده می‌شوند که شامل:
    ///   • Success : نشان‌دهنده موفقیت یا شکست عملیات
    ///   • Message : پیام فارسی قابل‌خواندن برای کاربر
    ///   • Data : نتیجه نهایی یا Null در حالت خطا
    /// ===========================================================================
    /// </summary>
    public class TaskService : ITaskService
    {
        // =======================================================================================
        // 🔹 وابستگی‌های داخلی سرویس - تزریق از بیرون
        // ---------------------------------------------------------------------------------------
        //   1️⃣ IAppDbContext   → اینترفیس انتزاعی دیتابیس (EF در لایه Persistence)
        //   2️⃣ IMapper         → نگاشت بین Entity و DTO
        //   3️⃣ TaskDtoValidator → اعتبارسنجی داده‌های ورودی با FluentValidation
        // =======================================================================================
        private readonly IAppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly TaskDtoValidator _validator;

        // =======================================================================================
        // 🏗️ سازنده سرویس
        // ---------------------------------------------------------------------------------------
        // وابستگی‌ها توسط DI تزریق می‌شوند تا تست‌پذیری و جداسازی معماری حفظ شود.
        // =======================================================================================
        public TaskService(IAppDbContext dbContext, IMapper mapper, TaskDtoValidator validator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _validator = validator;
        }

        // =======================================================================================
        // 🟩 CreateTaskAsync - ایجاد تسک جدید
        // ---------------------------------------------------------------------------------------
        // مراحل:
        //   1️⃣ اعتبارسنجی داده با Validator
        //   2️⃣ نگاشت DTO → Entity
        //   3️⃣ ذخیره در دیتابیس
        //   4️⃣ تولد خروجی ResponseModel<TaskDto>
        // =======================================================================================
        public async Task<ResponseModel<TaskDto>> CreateTaskAsync(TaskDto dto)
        {
            ValidationResult result = _validator.Validate(dto);
            if (!result.IsValid)
            {
                string errors = string.Join(" ؛ ", result.Errors.ConvertAll(e => e.ErrorMessage));
                return ResponseModel<TaskDto>.Fail($"داده‌ها معتبر نیستند: {errors}");
            }

            var entity = _mapper.Map<TaskEntity>(dto);
            _dbContext.Tasks.Add(entity);
            await _dbContext.SaveChangesAsync();

            var createdDto = _mapper.Map<TaskDto>(entity);
            return ResponseModel<TaskDto>.Ok("تسک با موفقیت ایجاد شد.", createdDto);
        }

        // =======================================================================================
        // 🟦 GetAllTasksAsync - واکشی تمام تسک‌ها
        // ---------------------------------------------------------------------------------------
        // رکوردهای جدول واکشی، مرتب‌سازی و نگاشت به لیست DTO انجام می‌شود.
        // =======================================================================================
        public async Task<ResponseModel<List<TaskDto>>> GetAllTasksAsync()
        {
            var entities = await _dbContext.Tasks
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            var dtos = _mapper.Map<List<TaskDto>>(entities);
            return ResponseModel<List<TaskDto>>.Ok("تمام تسک‌ها با موفقیت واکشی شدند.", dtos);
        }

        // =======================================================================================
        // 🟨 GetTaskByIdAsync - واکشی تسک بر اساس شناسه
        // ---------------------------------------------------------------------------------------
        // اگر تسک یافت نشود، پاسخ Fail برگشت داده می‌شود.
        // =======================================================================================
        public async Task<ResponseModel<TaskDto>> GetTaskByIdAsync(int id)
        {
            var entity = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null)
                return ResponseModel<TaskDto>.Fail($"تسک با شناسه {id} یافت نشد.");

            var dto = _mapper.Map<TaskDto>(entity);
            return ResponseModel<TaskDto>.Ok("تسک با موفقیت یافت شد.", dto);
        }

        // =======================================================================================
        // 🟧 UpdateTaskAsync - بروزرسانی اطلاعات تسک (اصلاح‌شده با شناسه جداگانه)
        // ---------------------------------------------------------------------------------------
        // داده‌ی جدید دریافت می‌شود، اعتبارسنجی و سپس بروزرسانی انجام می‌گردد.
        // =======================================================================================
        public async Task<ResponseModel<TaskDto>> UpdateTaskAsync(int id, TaskDto dto)
        {
            // ⚙️ ۱️⃣ واکشی رکورد هدف با شناسه مشخص
            var entity = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null)
                return ResponseModel<TaskDto>.Fail($"تسک با شناسه {id} یافت نشد.");

            // ⚙️ ۲️⃣ اعتبارسنجی ورودی جدید
            ValidationResult result = _validator.Validate(dto);
            if (!result.IsValid)
            {
                string errors = string.Join(" ؛ ", result.Errors.ConvertAll(e => e.ErrorMessage));
                return ResponseModel<TaskDto>.Fail($"داده‌های وارد شده معتبر نیستند: {errors}");
            }

            // ⚙️ ۳️⃣ نگاشت داده‌های جدید روی موجودیت فعلی
            _mapper.Map(dto, entity);

            // بروزرسانی زمان آخرین تغییر
            entity.UpdatedAt = dto.UpdatedAt ?? DateTime.Now.ToString("yyyy/MM/dd");

            // ⚙️ ۴️⃣ ذخیره در دیتابیس
            _dbContext.Tasks.Update(entity);
            await _dbContext.SaveChangesAsync();

            // ⚙️ ۵️⃣ ساخت DTO خروجی برای نمایش
            var updatedDto = _mapper.Map<TaskDto>(entity);
            return ResponseModel<TaskDto>.Ok("تسک با موفقیت بروزرسانی شد.", updatedDto);
        }

        // =======================================================================================
        // 🟥 DeleteTaskAsync - حذف تسک
        // ---------------------------------------------------------------------------------------
        // حذف رکورد بر اساس شناسه و برگشت پاسخ موفق یا خطا.
        // =======================================================================================
        public async Task<ResponseModel<object>> DeleteTaskAsync(int id)
        {
            var entity = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null)
                return ResponseModel<object>.Fail($"تسک با شناسه {id} یافت نشد.");

            _dbContext.Tasks.Remove(entity);
            await _dbContext.SaveChangesAsync();

            return ResponseModel<object>.Ok("تسک با موفقیت حذف شد.", null);
        }
    }
}
