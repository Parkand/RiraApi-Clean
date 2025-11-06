using Microsoft.EntityFrameworkCore;
using Rira.Application.Common;
using Rira.Application.Common.Exceptions;
using Rira.Application.DTOs;
using Rira.Application.Interfaces;
using Rira.Domain.Entities;

namespace Rira.Application.Services
{
    // =============================================================
    // 🧩 سرویس کارمندان - EmployeeService
    // نسخه: RiraDocs-v2025.11.5-Stable-Guid-Migration
    // -------------------------------------------------------------
    //  ☐ هماهنگ با IEmployeeService
    //  ☐ استفاده از ResponseModel<T>.Ok / .Fail
    //  ☐ پشتیبانی کامل از Guid و Validate داخلی
    // =============================================================
    public class EmployeeService : IEmployeeService
    {
        private readonly IAppDbContext _context;

        public EmployeeService(IAppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 📘 دریافت تمامی کارمندان
        // ============================================================
        public async Task<ResponseModel<List<EmployeeDTO>>> GetAllAsync()
        {
            try
            {
                var entities = await _context.Employees.ToListAsync();

                var dtos = entities.Select(e => new EmployeeDTO
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Gender = (EmployeeDTO.GenderType)e.Gender,
                    EducationLevel = (EmployeeDTO.EducationLevelType)e.EducationLevel,
                    FieldOfStudy = e.FieldOfStudy,
                    MobileNumber = e.MobileNumber,
                    BirthDatePersian = e.BirthDatePersian,
                    Position = e.Position,
                    Email = e.Email,
                    HireDate = e.HireDate,
                    IsActive = e.IsActive,
                    Description = e.Description
                }).ToList();

                return ResponseModel<List<EmployeeDTO>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseModel<List<EmployeeDTO>>.Fail($"خطایی در انجام عملیات رخ داد: {ex.Message}");
            }
        }

        // ============================================================
        // 📘 دریافت کارمند بر اساس Guid
        // ============================================================
        public async Task<ResponseModel<EmployeeDTO>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
                if (entity is null)
                    throw new NotFoundException($"کارمند با شناسه {id} یافت نشد.");

                var dto = new EmployeeDTO
                {
                    Id = entity.Id,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Gender = (EmployeeDTO.GenderType)entity.Gender,
                    EducationLevel = (EmployeeDTO.EducationLevelType)entity.EducationLevel,
                    FieldOfStudy = entity.FieldOfStudy,
                    MobileNumber = entity.MobileNumber,
                    BirthDatePersian = entity.BirthDatePersian,
                    Position = entity.Position,
                    Email = entity.Email,
                    HireDate = entity.HireDate,
                    IsActive = entity.IsActive,
                    Description = entity.Description
                };

                return ResponseModel<EmployeeDTO>.Ok(dto);
            }
            catch (NotFoundException nf)
            {
                return ResponseModel<EmployeeDTO>.Fail(nf.Message, 404);
            }
            catch (Exception ex)
            {
                return ResponseModel<EmployeeDTO>.Fail($"خطا در واکشی داده‌ها: {ex.Message}");
            }
        }

        // ============================================================
        // 📘 ایجاد کارمند جدید
        // ============================================================
        public async Task<ResponseModel<EmployeeDTO>> CreateAsync(EmployeeDTO dto)
        {
            try
            {
                var entity = new EmployeeEntity
                {
                    Id = dto.Id != Guid.Empty ? dto.Id : Guid.NewGuid(),
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Gender = (EmployeeEntity.GenderType)dto.Gender,
                    EducationLevel = (EmployeeEntity.EducationLevelType)dto.EducationLevel,
                    FieldOfStudy = dto.FieldOfStudy,
                    MobileNumber = dto.MobileNumber,
                    BirthDatePersian = dto.BirthDatePersian,
                    Position = dto.Position,
                    Email = dto.Email,
                    HireDate = dto.HireDate,
                    IsActive = dto.IsActive,
                    Description = dto.Description
                };

                entity.Validate();
                _context.Employees.Add(entity);
                await _context.SaveChangesAsync();

                dto.Id = entity.Id;
                return ResponseModel<EmployeeDTO>.Ok(dto);
            }
            catch (RiraValidationException ve)
            {
                return ResponseModel<EmployeeDTO>.Fail($"اعتبارسنجی ناموفق بود: {ve.Message}", 400);
            }
            catch (Exception ex)
            {
                return ResponseModel<EmployeeDTO>.Fail($"خطا در ایجاد کارمند: {ex.Message}");
            }
        }

        // ============================================================
        // 📘 بروزرسانی اطلاعات کارمند
        // ============================================================
        public async Task<ResponseModel<EmployeeDTO>> UpdateAsync(EmployeeDTO dto)
        {
            try
            {
                var entity = await _context.Employees.FirstOrDefaultAsync(e => e.Id == dto.Id);
                if (entity is null)
                    throw new NotFoundException($"کارمند با شناسه {dto.Id} یافت نشد.");

                entity.FirstName = dto.FirstName;
                entity.LastName = dto.LastName;
                entity.Gender = (EmployeeEntity.GenderType)dto.Gender;
                entity.EducationLevel = (EmployeeEntity.EducationLevelType)dto.EducationLevel;
                entity.FieldOfStudy = dto.FieldOfStudy;
                entity.MobileNumber = dto.MobileNumber;
                entity.BirthDatePersian = dto.BirthDatePersian;
                entity.Position = dto.Position;
                entity.Email = dto.Email;
                entity.HireDate = dto.HireDate;
                entity.IsActive = dto.IsActive;
                entity.Description = dto.Description;

                entity.Validate();
                await _context.SaveChangesAsync();

                return ResponseModel<EmployeeDTO>.Ok(dto);
            }
            catch (NotFoundException nf)
            {
                return ResponseModel<EmployeeDTO>.Fail(nf.Message, 404);
            }
            catch (RiraValidationException ve)
            {
                return ResponseModel<EmployeeDTO>.Fail($"خطای اعتبارسنجی: {ve.Message}", 400);
            }
            catch (Exception ex)
            {
                return ResponseModel<EmployeeDTO>.Fail($"خطا در بروزرسانی: {ex.Message}");
            }
        }

        // ============================================================
        // 📘 حذف کارمند بر اساس Guid
        // ============================================================
        public async Task<ResponseModel<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
                if (entity is null)
                    throw new NotFoundException($"کارمند با شناسه {id} یافت نشد.");

                _context.Employees.Remove(entity);
                await _context.SaveChangesAsync();

                return ResponseModel<bool>.Ok(true);
            }
            catch (NotFoundException nf)
            {
                return ResponseModel<bool>.Fail(nf.Message, 404);
            }
            catch (Exception ex)
            {
                return ResponseModel<bool>.Fail($"خطا در حذف: {ex.Message}");
            }
        }

        // ============================================================
        // 📘 بررسی تکراری بودن ایمیل و شماره موبایل
        // ============================================================
        public async Task<bool> IsEmailDuplicateAsync(string email)
            => await _context.Employees.AnyAsync(e => e.Email == email);

        public async Task<bool> IsMobileDuplicateAsync(string mobileNumber)
            => await _context.Employees.AnyAsync(e => e.MobileNumber == mobileNumber);
    }
}
