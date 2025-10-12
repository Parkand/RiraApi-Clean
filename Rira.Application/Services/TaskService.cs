using AutoMapper;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.DTOs;
using Rira.Application.Interfaces;
using Rira.Application.Validators;
using Rira.Domain.Entities;
using System.Net;

namespace Rira.Application.Services
{
    /// <summary>
    /// ===========================================================================
    /// ✅ سرویس اصلی مدیریت تسک‌ها (TaskService)
    /// ---------------------------------------------------------------------------
    /// این کلاس در لایه‌ی Application قرار دارد و از IAppDbContext برای تعامل با داده‌ها استفاده می‌کند.
    /// تمام خروجی‌ها در قالب ResponseModel<T> برگردانده می‌شوند که متدهای کمکی شامل:
    ///   - Ok(...)        → عملیات موفق با کد 200
    ///   - Fail(...)      → عملیات ناموفق با پیام خطا
    ///   - NotFound(...)  → برای رکوردهای پیدا نشد
    /// ===========================================================================
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly IAppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly TaskDtoValidator _validator;

        public TaskService(IAppDbContext dbContext, IMapper mapper, TaskDtoValidator validator)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _validator = validator;
        }

        // =======================================================================================
        // 🟩 CreateTaskAsync - ایجاد تسک جدید
        // =======================================================================================
        public async Task<ResponseModel<int>> CreateTaskAsync(TaskDto dto)
        {
            ValidationResult validation = _validator.Validate(dto);
            if (!validation.IsValid)
            {
                string errors = string.Join(" ؛ ", validation.Errors.ConvertAll(e => e.ErrorMessage));
                return ResponseModel<int>.Fail($"داده‌ها معتبر نیستند: {errors}", (int)HttpStatusCode.BadRequest);
            }

            var entity = _mapper.Map<TaskEntity>(dto);
            await _dbContext.Tasks.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return ResponseModel<int>.Ok(entity.Id, "✅ تسک با موفقیت ایجاد شد.");
        }

        // =======================================================================================
        // 🟦 GetAllTasksAsync - واکشی تمام تسک‌ها
        // =======================================================================================
        public async Task<ResponseModel<List<TaskDto>>> GetAllTasksAsync()
        {
            var entities = await _dbContext.Tasks
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            var dtos = _mapper.Map<List<TaskDto>>(entities);
            return ResponseModel<List<TaskDto>>.Ok(dtos, "✅ تمام تسک‌ها با موفقیت واکشی شدند.");
        }

        // =======================================================================================
        // 🟨 GetTaskByIdAsync - واکشی تسک بر اساس شناسه
        // =======================================================================================
        public async Task<ResponseModel<TaskDto>> GetTaskByIdAsync(int id)
        {
            var entity = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (entity == null)
                return ResponseModel<TaskDto>.NotFound($"❌ تسک با شناسه {id} یافت نشد.");

            var dto = _mapper.Map<TaskDto>(entity);
            return ResponseModel<TaskDto>.Ok(dto, "✅ تسک با موفقیت یافت شد.");
        }

        // =======================================================================================
        // 🟧 UpdateTaskAsync - بروزرسانی اطلاعات تسک
        // =======================================================================================
        public async Task<ResponseModel<int>> UpdateTaskAsync(int id, TaskDto dto)
        {
            var entity = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
            if (entity == null)
                return ResponseModel<int>.NotFound($"❌ تسک با شناسه {id} یافت نشد.");

            ValidationResult validation = _validator.Validate(dto);
            if (!validation.IsValid)
            {
                string errors = string.Join(" ؛ ", validation.Errors.ConvertAll(e => e.ErrorMessage));
                return ResponseModel<int>.Fail($"داده‌های وارد شده معتبر نیستند: {errors}", (int)HttpStatusCode.BadRequest);
            }

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.Now.ToString("yyyy/MM/dd");

            _dbContext.Tasks.Update(entity);
            await _dbContext.SaveChangesAsync();

            return ResponseModel<int>.Ok(entity.Id, "✅ تسک با موفقیت بروزرسانی شد.");
        }

        // =======================================================================================
        // 🟥 DeleteTaskAsync - حذف نرم (Soft Delete)
        // =======================================================================================
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
