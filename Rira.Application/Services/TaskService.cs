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
    /// <summary>
    /// ===============================================================================
    /// ✅ سرویس اصلی مدیریت تسک‌ها (TaskService)
    /// -------------------------------------------------------------------------------
    /// این کلاس در لایه‌ی Application قرار دارد و وظیفه‌ی اجرای منطق کسب‌وکار مربوط 
    /// به موجودیت «Task» را برعهده دارد. ارتباط با داده‌ها از طریق IAppDbContext انجام می‌شود.
    /// تمام خروجی‌ها به شکل ResponseModel<T> برگردانده می‌شوند تا کنترل وضعیت‌ها، 
    /// پیام‌ها و کدهای HTTP به‌صورت شفاف انجام گیرد.
    /// 
    /// تزریق وابستگی‌ها:
    ///   🟩 IAppDbContext   → دسترسی به DbSet<TaskEntity>
    ///   🟦 IMapper         → نگاشت بین TaskDto ↔ TaskEntity
    ///   🟨 IValidator<TaskDto> → اعتبار‌سنجی داده‌های ورودی در سطح DTO
    /// 
    /// ===============================================================================
    /// </summary>
    public class TaskService : ITaskService
    {
        // ============================================================
        // 🧩 وابستگی‌ها (Dependency Injection)
        // ============================================================
        private readonly IAppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<TaskDto> _validator;

        /// <summary>
        /// سازنده‌ی اصلی سرویس با تزریق وابستگی‌های مورد نیاز.
        /// </summary>
        public TaskService(IAppDbContext dbContext, IMapper mapper, IValidator<TaskDto> validator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _validator = validator;
        }

        // ============================================================
        // 🟩 CreateTaskAsync
        // ------------------------------------------------------------
        // ایجاد تسک جدید بر اساس DTO ورودی، با اعتبارسنجی و نگاشت به Entity.
        // ============================================================
        public async Task<ResponseModel<int>> CreateTaskAsync(TaskDto dto)
        {
            // 🧪 اعتبارسنجی ورودی توسط Validator
            ValidationResult validation = _validator.Validate(dto);
            if (!validation.IsValid)
            {
                string errors = string.Join(" ؛ ", validation.Errors.ConvertAll(e => e.ErrorMessage));
                return ResponseModel<int>.Fail($"داده‌ها معتبر نیستند: {errors}", (int)HttpStatusCode.BadRequest);
            }

            // 🧩 نگاشت DTO → Entity
            var entity = _mapper.Map<TaskEntity>(dto);

            await _dbContext.Tasks.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return ResponseModel<int>.Ok(entity.Id, "✅ تسک با موفقیت ایجاد شد.");
        }

        // ============================================================
        // 🟦 GetAllTasksAsync
        // ------------------------------------------------------------
        // واکشی تمام تسک‌ها از پایگاه داده با فیلتر SoftDelete.
        // ============================================================
        public async Task<ResponseModel<List<TaskDto>>> GetAllTasksAsync()
        {
            // واکشی از DbContext
            var entities = await _dbContext.Tasks
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            // نگاشت Entity → DTO
            var dtos = _mapper.Map<List<TaskDto>>(entities);

            return ResponseModel<List<TaskDto>>.Ok(dtos, "✅ تمام تسک‌ها با موفقیت واکشی شدند.");
        }

        // ============================================================
        // 🟨 GetTaskByIdAsync
        // ------------------------------------------------------------
        // واکشی تسک واحد بر اساس شناسه (با بررسی SoftDelete).
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
        // بروزرسانی اطلاعات تسک بر اساس شناسه و داده‌های DTO.
        // ============================================================
        public async Task<ResponseModel<int>> UpdateTaskAsync(int id, TaskDto dto)
        {
            var entity = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (entity == null)
                return ResponseModel<int>.NotFound($"❌ تسک با شناسه {id} یافت نشد.");

            // 🧪 اعتبارسنجی ورودی
            ValidationResult validation = _validator.Validate(dto);
            if (!validation.IsValid)
            {
                string errors = string.Join(" ؛ ", validation.Errors.ConvertAll(e => e.ErrorMessage));
                return ResponseModel<int>.Fail($"داده‌های وارد شده معتبر نیستند: {errors}", (int)HttpStatusCode.BadRequest);
            }

            // 🧩 نگاشت DTO → Entity (برای بروزرسانی)
            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.Now.ToString("yyyy/MM/dd");

            _dbContext.Tasks.Update(entity);
            await _dbContext.SaveChangesAsync();

            return ResponseModel<int>.Ok(entity.Id, "✅ تسک با موفقیت بروزرسانی شد.");
        }

        // ============================================================
        // 🟥 DeleteTaskAsync
        // ------------------------------------------------------------
        // حذف نرم (Soft Delete) بر اساس شناسه، بدون حذف فیزیکی.
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
}
