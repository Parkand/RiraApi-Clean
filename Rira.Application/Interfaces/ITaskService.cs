using Rira.Application.Common;
using Rira.Application.DTOs;

namespace Rira.Application.Interfaces
{
    // 🎯 رابط سرویس مدیریتی تسک‌ها — ITaskService
    // -------------------------------------------------------------------
    // این Interface نشان‌دهنده‌ی "لایه‌ی سرویس برنامه کاربردی" (Application Service Layer)
    // در معماری تمیز (Clean Architecture) پروژه‌ی ریرا است.
    //
    // 📌 هدف کلی:
    //     ▫ حفظ جداسازی منطق تجاری از زیرساخت ذخیره‌سازی.
    //     ▫ پیاده‌سازی استانداردها برای دسترسی به داده‌ها بدون وابستگی مستقیم به EF Core.
    //     ▫ پشتیبانی از تزریق وابستگی (DI) در کلاس TaskService.
    //     ▫ تسهیل تست واحد (Unit Test) و تست یکپارچه (Integration Test).
    //
    // ⚙️ کلیه‌ی خروجی‌های متدها از نوع ResponseModel هستند:
    //     این مدل ساختار استاندارد پاسخ در سراسر پروژه است و شامل:
    //       • Success  → وضعیت موفقیت عملیات.
    //       • Message  → پیام فارسی خوانا و کاربر‌پسند.
    //       • Data     → خروجی از نوع داده/شناسه.
    //
    // 🧠 لایه‌ی TaskService در RiRa به‌عنوان مرز بین Command/Handler و Controller‌ها عمل می‌کند،
    //     و منطق ترکیبی (مثل SoftDelete یا اعتبارسنجی چندمرحله‌ای) را مدیریت می‌کند.
    public interface ITaskService
    {
        // 🟩 دریافت تمام تسک‌ها — Read All
        // -------------------------------------------------------------------
        // این متد تمام رکوردهای فعال Task را واکشی کرده و در قالب TaskDto خروجی می‌دهد.
        // ⚙️ عملیات معمول:
        //     - واکشی از Repository
        //     - تبدیل با AutoMapper
        //     - بسته‌بندی در ResponseModel<List<TaskDto>>
        Task<ResponseModel<List<TaskDto>>> GetAllTasksAsync();

        // 🟨 دریافت یک تسک بر اساس شناسه — Read By Id
        // -------------------------------------------------------------------
        // این متد وظیفه‌ی بازیابی یک Task بر اساس Id را دارد.
        // ⚙️ در پیاده‌سازی:
        //     - ابتدا TaskRepository.GetByIdAsync(id)
        //     - سپس بررسی وجود داده و تبدیل به DTO توسط Mapper
        //     - در خروجی ResponseModel<TaskDto>
        Task<ResponseModel<TaskDto>> GetTaskByIdAsync(int id);

        // 🟧 ایجاد تسک جدید — Create
        // -------------------------------------------------------------------
        // این متد برای ساخت رکورد جدید TaskEntity استفاده می‌شود.
        // ⚙️ فرایند معمول:
        //     - دریافت DTO از Controller
        //     - اعتبارسنجی داده‌ها توسط Validator
        //     - افزودن از طریق Repository.CreateAsync()
        //     - بازگرداندن Id رکورد ایجادشده
        Task<ResponseModel<int>> CreateTaskAsync(TaskDto dto);

        // 🟦 بروزرسانی تسک موجود — Update
        // -------------------------------------------------------------------
        // این متد رکورد موجود را با داده‌های جدید بروزرسانی می‌کند.
        // ⚙️ مراحل:
        //     - بررسی وجود رکورد با GetByIdAsync
        //     - بروزرسانی فیلدهای مرتبط
        //     - ذخیره در Repository.UpdateAsync()
        //     - بازگرداندن Id و پیام موفقیت
        Task<ResponseModel<int>> UpdateTaskAsync(int id, TaskDto dto);

        // 🟥 حذف نرم (Soft Delete) تسک — Delete
        // -------------------------------------------------------------------
        // متد مربوط به غیرفعال‌سازی منطقی رکورد است، نه حذف فیزیکی.
        // ⚙️ در پیاده‌سازی:
        //     - علامت IsDeleted = true و تاریخ حذف DeletedAt ثبت می‌شود.
        //     - سپس UpdateAsync برای ذخیره تغییرات فراخوانی می‌شود.
        // ⚙️ خروجی:
        //     - شناسه‌ی رکورد حذف‌شده ✅
        //     - پیام موفقیت‌آمیز 💬
        Task<ResponseModel<int>> DeleteTaskAsync(int id);
    }

    // ===========================================================================================
    // 📘 خلاصه آموزشی (RiraDocs Teaching Edition)
    // -------------------------------------------------------------------------------------------
    // 🔹 نقش در معماری RiRa:
    //     ▫ سطح Application Service، بالاتر از Repository، پایین‌تر از Controller.
    //     ▫ تلفیق داده، اعتبارسنجی، و کنترل منطق خاص دامنه.
    //
    // 🔹 وابستگی‌ها:
    //     ▫ ITaskRepository → برای عملیات CRUD.
    //     ▫ IAppDbContext   → برای مدیریت ترنزکشن‌ها.
    //     ▫ DTOها و ResponseModel → برای تبادل منظم داده بین لایه‌ها.
    //
    // 🔹 قابلیت تست:
    //     ▫ امکان ساخت Mock از ITaskService برای تست کنترلرها بدون دسترسی دیتابیس واقعی.
    //
    // 🔹 رعایت اصول:
    //     ▫ Dependency Inversion Principle (DIP)
    //     ▫ Separation of Concerns (SoC)
    //     ▫ Asynchronous Programming Best Practices
    //
    // 🔹 تگ انتشار RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===========================================================================================
}
