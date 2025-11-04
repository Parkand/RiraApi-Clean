using AutoMapper;
using Rira.Domain.Entities;
using Rira.Application.DTOs;

namespace Rira.Application.MappingProfiles
{
    // 🧩 پروفایل نگاشت AutoMapper برای مدل Employee
    // -------------------------------------------------------------------
    // این کلاس مسئول نگاشت (Mapping) بین موجودیت دامنه (Domain Entity)
    // و مدل انتقال داده (DTO) مربوط به موجودیت کارمند است.
    //
    // 🎯 هدف:
    //     ▫ جداسازی منطق نگاشت از کلاس‌های دامنه و DTO.
    //     ▫ عدم آمیختگی با منطق تجاری — فقط تبدیل داده.
    //     ▫ ساده‌سازی فرایند انتقال داده بین لایه‌های Application و Domain.
    //
    // ⚙️ جایگاه در معماری Clean Architecture RiRa:
    //     ▫ در پوشه‌ی MappingProfiles از لایه‌ی Application.
    //     ▫ AutoMapper در startup با AddAutoMapper(typeof(EmployeeProfile)) تنظیم می‌شود.
    //
    // 🔹 نکته‌ی آموزشی:
    //     AutoMapper از مفهوم "Profile" استفاده می‌کند تا نگاشت‌های مرتبط در یک کلاس منسجم قرار گیرند.
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            // ============================================================
            // ✅ نگاشت از Domain به DTO (Entity → DTO)
            // ============================================================
            // در این بخش‌، مشخص می‌کنیم داده‌های از نوع EmployeeEntity چگونه
            // به مدل DTO (EmployeeDTO) تبدیل شوند.

            CreateMap<EmployeeEntity, EmployeeDTO>()
                // 🧠 ترکیب نام و نام خانوادگی برای تشکیل FullName خروجی
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))

                // 🔄 تبدیل Enumهای دامنه به Enumهای DTO
                // در صورت تفاوت تعریف یا Namespace جدا بین Domain و DTO لازم است.
                .ForMember(dest => dest.Gender,
                    opt => opt.MapFrom(src => (EmployeeDTO.GenderType)(int)src.Gender))
                .ForMember(dest => dest.EducationLevel,
                    opt => opt.MapFrom(src => (EmployeeDTO.EducationLevelType)(int)src.EducationLevel));

            // ============================================================
            // ✅ نگاشت از DTO به Domain (DTO → Entity)
            // ============================================================
            // در این قسمت مشخص می‌شود داده‌ای که از Controller/Service
            // به صورت DTO دریافت شده چگونه باید به Entity تبدیل شود
            // تا در دیتابیس ذخیره گردد.

            CreateMap<EmployeeDTO, EmployeeEntity>()
                // ⚠ FullName فقط خروجی محاسباتی است و نباید به Entity برگردانده شود.
                .ForMember(dest => dest.FullName, opt => opt.Ignore())

                // 🔄 تبدیل Enumهای DTO به Enumهای Domain بازگشتی.
                .ForMember(dest => dest.Gender,
                    opt => opt.MapFrom(src => (EmployeeEntity.GenderType)(int)src.Gender))
                .ForMember(dest => dest.EducationLevel,
                    opt => opt.MapFrom(src => (EmployeeEntity.EducationLevelType)(int)src.EducationLevel));
        }
    }

    // ===========================================================================================
    // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
    // -------------------------------------------------------------------------------------------
    // 🔹 نقش EmployeeProfile:
    //     ▫ تسهیل تبدیل داده بین موجودیت و DTO برای Controllerها و Serviceها.
    //     ▫ جلوگیری از تکرار کد Mapper در Handlerها و Commandها.
    //
    // 🔹 چرخه‌ی استفاده:
    //     Entity ↔ DTO ↔ Controller
    //     (از طریق DI و AutoMapper)
    //
    // 🔹 مزیت معماری:
    //     ▫ نگهداری تمیزتر داده‌ها و Open/Closed Principle در SOLID.
    //     ▫ تغییرات در ساختار DTO یا Entity تنها با اصلاح Profile مدیریت می‌شود.
    //
    // 🔹 آزمون‌پذیری:
    //     ▫ می‌توان تست واحد برای صحت نگاشت‌ها با AssertConfigurationIsValid() نوشت.
    //
    // 🔹 تگ انتشار RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===========================================================================================
}
