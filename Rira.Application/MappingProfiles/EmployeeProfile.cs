using AutoMapper;
using Rira.Domain.Entities;
using Rira.Application.DTOs;
using static Rira.Application.DTOs.EmployeeDTO;

namespace Rira.Application.MappingProfiles
{
    /// <summary>
    /// 🧩 پروفایل AutoMapper برای مدل «Employee»
    /// ----------------------------------------------------
    /// این کلاس یکی از بخش‌های کلیدی لایه‌ی Application محسوب می‌شود
    /// و وظیفه‌اش نگاشت (Mapping) خودکار بین مدل دامنه‌ی EmployeeEntity
    /// و مدل انتقال داده‌ی EmployeeDTO است.
    ///
    /// ➤ هدف اصلی:
    /// تبدیل خودکار داده‌ها بین Entity و DTO بدون دخالت منطق تجاری یا اعتبارسنجی.
    /// به کمک AutoMapper، بار تبدیل دستی فیلدها کاهش یافته و یکپارچگی داده بین لایه‌ها حفظ می‌شود.
    ///
    /// ⚙️ رفتار این Profile دقیقاً مطابق اصول Clean Architecture:
    ///   - Domain آزاد از وابستگی‌های Application.
    ///   - Application فقط وظیفه‌ی نگاشت و بازنمایی داده را دارد.
    /// </summary>
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            // ============================================================
            // ✅ نگاشت از Domain به DTO (Entity → DTO)
            // ============================================================
            CreateMap<EmployeeEntity, EmployeeDTO>()
                // 🔸 تولید نام کامل از ترکیب نام و نام‌خانوادگی (خواندنی فقط از سمت خروجی)
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))

                // 🔸 نگاشت Enum جنسیت از Domain به DTO (تبدیل عددی جهت سازگاری دو Enum داخلی)
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => (GenderType)(int)src.Gender))

                // 🔸 نگاشت Enum سطح تحصیلات
                .ForMember(dest => dest.EducationLevel, opt => opt.MapFrom(src => (EducationLevelType)(int)src.EducationLevel))

                // 🔸 فعال‌سازی نگاشت دوطرفه (ReverseMap) بین Entity ↔ DTO
                // بدین شکل که همان تنظیمات بالا در جهت عکس نیز برقرار می‌شود.
                .ReverseMap()

                // ============================================================
                // ✅ نگاشت از DTO به Domain (DTO → Entity)
                // ============================================================
                // هر فیلد به‌صورت مستقیم بازنگاشت می‌شود تا عملیات ذخیره و بروزرسانی در DB بدون مشکل انجام شود.
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Gender,
                    opt => opt.MapFrom(src => (EmployeeEntity.GenderType)(int)src.Gender))
                .ForMember(dest => dest.EducationLevel,
                    opt => opt.MapFrom(src => (EmployeeEntity.EducationLevelType)(int)src.EducationLevel))
                .ForMember(dest => dest.FieldOfStudy, opt => opt.MapFrom(src => src.FieldOfStudy))
                .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.MobileNumber))
                .ForMember(dest => dest.BirthDatePersian, opt => opt.MapFrom(src => src.BirthDatePersian))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.HireDate, opt => opt.MapFrom(src => src.HireDate))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));
            // تمام فیلدهای ساختاری و اطلاعاتی از DTO به Domain به‌طور مستقیم نگاشت می‌شوند.
        }
    }
}
