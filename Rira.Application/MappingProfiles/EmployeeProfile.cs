using AutoMapper;
using Rira.Domain.Entities;
using Rira.Application.DTOs;

namespace Rira.Application.MappingProfiles
{
    /// <summary>
    /// 🧩 پروفایل نگاشت AutoMapper برای مدل Employee
    /// وظیفه‌ی تبدیل بین Entity و DTO را بدون منطق تجاری انجام می‌دهد.
    /// </summary>
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            // ============================================================
            // ✅ نگاشت از Domain به DTO (Entity → DTO)
            // ============================================================
            CreateMap<EmployeeEntity, EmployeeDTO>()
                // ترکیب نام و نام خانوادگی به عنوان FullName
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))

                // تبدیل Enum های دامنه به Enum های DTO (در صورت تفاوت تعریف)
                .ForMember(dest => dest.Gender,
                    opt => opt.MapFrom(src => (EmployeeDTO.GenderType)(int)src.Gender))
                .ForMember(dest => dest.EducationLevel,
                    opt => opt.MapFrom(src => (EmployeeDTO.EducationLevelType)(int)src.EducationLevel));

            // ============================================================
            // ✅ نگاشت از DTO به Domain (DTO → Entity)
            // ============================================================
            CreateMap<EmployeeDTO, EmployeeEntity>()
                // FullName نباید به دامنه برگردانده شود
                .ForMember(dest => dest.FullName, opt => opt.Ignore())

                .ForMember(dest => dest.Gender,
                    opt => opt.MapFrom(src => (EmployeeEntity.GenderType)(int)src.Gender))
                .ForMember(dest => dest.EducationLevel,
                    opt => opt.MapFrom(src => (EmployeeEntity.EducationLevelType)(int)src.EducationLevel));
        }
    }
}
