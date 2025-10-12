using AutoMapper;
using Rira.Application.DTOs;
using Rira.Domain.Entities;
using Rira.Application.Utilities;
using TaskStatus = Rira.Domain.Entities.TaskStatus;

namespace Rira.Application.MappingProfiles
{
    /// <summary>
    /// 🎯 پروفایل AutoMapper مخصوص تسک‌ها
    /// مپ دوطرفه بین TaskEntity ↔ TaskDto به‌صورت پایدار و امن
    /// شامل تبدیل Enumها و تاریخ شمسی
    /// </summary>
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            // ✅ از Entity به DTO
            CreateMap<TaskEntity, TaskDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ReverseMap() // فعال‌سازی مپ دوطرفه
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ParseStatus(src.Status)))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => ParsePriority(src.Priority)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => NormalizeDate(src.CreatedAt)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => NormalizeDate(src.UpdatedAt)));
        }

        // 🔹 تبدیل رشته به Enum Status
        private static TaskStatus ParseStatus(string? value)
        {
            if (Enum.TryParse<TaskStatus>(value, ignoreCase: true, out var parsed))
                return parsed;
            return TaskStatus.Pending; // مقدار پیش‌فرض در صورت نامعتبر بودن
        }

        // 🔹 تبدیل رشته به Enum Priority
        private static TaskPriority ParsePriority(string? value)
        {
            if (Enum.TryParse<TaskPriority>(value, ignoreCase: true, out var parsed))
                return parsed;
            return TaskPriority.Medium; // مقدار پیش‌فرض
        }

        // 🔹 نرمال‌سازی تاریخ شمسی (در صورت خالی بودن تولید تاریخ فعلی)
        private static string NormalizeDate(string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            return DateHelper.GetPersianNow();
        }
    }
}
