using AutoMapper;
using Rira.Application.DTOs;
using Rira.Application.Features.Tasks.Commands.Create;
using Rira.Application.Features.Tasks.Commands.Update;
using Rira.Domain.Entities;

namespace Rira.Application.MappingProfiles
{
    // 🧩 پروفایل نگاشت AutoMapper برای مدل Task
    // -------------------------------------------------------------------
    // این کلاس "TaskProfile" نقش نگاشت داده‌ها بین لایه‌های مختلف را دارد.
    // هدف اصلی، حفظ جداسازی Domain و Application و سهولت تبدیل داده‌ها
    // میان موجودیت‌ها (Entities)، دستورات (Commands) و DTOها است.
    //
    // 🎯 هدف کلی:
    //     ▫ کاهش کد تکراری Mapper در Handlerها و سرویس‌ها.
    //     ▫ انجام تبدیل داده‌ها به صورت خودکار و استاندارد.
    //     ▫ ساده‌سازی کار با MediatR Commands (ایجاد، بروزرسانی، خواندن).
    //
    // ⚙️ جایگاه در معماری RiRa:
    //     ▫ بخشی از پوشه‌ی Application/MappingProfiles.
    //     ▫ توسط DI در Program.cs ثبت می‌شود: AddAutoMapper(typeof(TaskProfile))
    //
    // 🧠 AutoMapper با مفهوم "Profile" اجازه تعریف نگاشت‌ها را در کلاس‌های مستقل می‌دهد.
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            // ============================================================
            // ✅ نگاشت بین Domain Entity و DTO  (Entity ↔ DTO)
            // ============================================================
            // این نگاشت وظیفه دارد مشخصات موجودیت TaskEntity را به Data Transfer Object تبدیل کند
            // و بالعکس — بدون دخالت در منطق تجاری یا دیتابیس.
            //
            // 🎓 کاربرد:
            //     ▫ در زمانی که داده‌ها باید از Repository گرفته و به‌صورت DTO در Controller نمایش داده شوند.
            //     ▫ یا وقتی در Handler پاسخ خالص از نوع TaskDto نیاز داریم.
            CreateMap<TaskEntity, TaskDto>().ReverseMap();

            // ============================================================
            // ✅ نگاشت بین Command و Domain  (CreateCommand ↔ Entity)
            // ============================================================
            // این نگاشت مسئول تبدیل داده‌های ورودی کامند ایجاد (CreateTaskCommand)
            // به مدل دامنه برای درج در دیتابیس است.
            //
            // ⚙️ جریان معمول:
            //     Controller → CreateTaskCommand → TaskEntity → SaveChangesAsync()
            //
            // ReverseMap باعث می‌شود نگاشت دوطرفه انجام شود
            // (در صورت نیاز برای برگرداندن داده موجودیت به Command).
            CreateMap<TaskCreateCommand, TaskEntity>().ReverseMap();

            // ============================================================
            // ✅ نگاشت بین Command و Domain  (UpdateCommand ↔ Entity)
            // ============================================================
            // این نگاشت مشابه مورد بالا است اما برای بروزرسانی رکورد‌های موجود در دیتابیس.
            // داده‌ی ورودی از TaskUpdateCommand گرفته می‌شود و به موجودیت دامنه نگاشت می‌گردد.
            //
            // ⚙️ جریان:
            //     Controller → UpdateTaskCommand → Handler → Entity → Update() → SaveChangesAsync()
            //
            // ReverseMap نیز برای سناریوهای خاص تست و ابزارهای داخلی استفاده می‌شود.
            CreateMap<TaskUpdateCommand, TaskEntity>().ReverseMap();
        }
    }

    // ===========================================================================================
    // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
    // -------------------------------------------------------------------------------------------
    // 🔹 نقش TaskProfile:
    //     ▫ پیونددهنده‌ی لایه‌های Command و DTO با Domain Entity.
    //     ▫ عنصر اصلی برای گردش داده بدون وابستگی به EF Core یا منطق داخلی Handler.
    //
    // 🔹 وابستگی‌ها:
    //     ▫ AutoMapper – برای نگاشت خودکار.
    //     ▫ Domain Entities / DTOها / Commands در ماژول Task.
    //
    // 🔹 مزایا:
    //     ▫ کد ایمن و قابل تست با AssertConfigurationIsValid().
    //     ▫ کاهش نیاز به Map دستی در Handlerها.
    //     ▫ رعایت اصل Single Responsibility و Open/Closed در SOLID.
    //
    // 🔹 تگ انتشار RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===========================================================================================
}
