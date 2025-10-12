using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using Rira.Application.Common.Exceptions;

namespace Rira.Tests.Application.Common.Exceptions
{
    /// <summary>
    /// ✅ تست واحد کامل برای کلاس RiraValidationException
    /// --------------------------------------------------------------
    /// این Exception مخصوص خطاهای اعتبارسنجی داده‌هاست (مثلاً داده ورودی کاربر نامعتبر باشد).
    /// هدف این تست‌ها اطمینان از عملکرد صحیح سازنده‌ها، تولید پیام، و ساختار لیست خطاها است.
    /// </summary>
    public class RiraValidationExceptionTests
    {
        // ======================================================================================================
        // 🔹 تست ۱: سازنده با لیست چندخطا
        // ------------------------------------------------------------------------------------------------------
        // بررسی کنیم که هنگام ارسال چند خطا، لیست Errors درست مقداردهی شود
        // و Message شامل همهٔ خطاها باشد.
        // ======================================================================================================
        [Fact]
        public void Constructor_Should_Set_Errors_And_Message_For_MultipleErrors()
        {
            // 🧩 آماده‌سازی داده‌ی تست
            var errorList = new List<string>
            {
                "عنوان خالی است",
                "توضیحات خیلی کوتاه است",
                "تاریخ نامعتبر است"
            };

            // 🧪 اجرای تست
            var exception = new RiraValidationException(errorList);

            // ✅ بررسی نتایج
            exception.Errors.Should().HaveCount(3, "باید دقیقاً همان تعداد خطاهایی که ارسال کردیم را نگه دارد");
            exception.Message.Should().Contain("Validation failed:", "پیام کلی باید با عبارت استاندارد آغاز شود.");
            exception.Message.Should().Contain("عنوان خالی است");
            exception.Message.Should().Contain("توضیحات خیلی کوتاه است");
            exception.Message.Should().Contain("تاریخ نامعتبر است");
        }

        // ======================================================================================================
        // 🔹 تست ۲: سازنده با یک خطا (singleError)
        // ------------------------------------------------------------------------------------------------------
        // بررسی کنیم که سازنده‌ی ساده با یک رشته نیز کار کند و همان خطا را در Errors ذخیره نماید.
        // ======================================================================================================
        [Fact]
        public void Constructor_Should_Work_With_SingleError_String()
        {
            // 🧩 دادهٔ تست
            string errorText = "عنوان اجباری است";

            // 🧪 اجرای تست
            var exception = new RiraValidationException(errorText);

            // ✅ بررسی‌ها
            exception.Errors.Should().BeEquivalentTo(new[] { errorText }, "در سازنده‌ی تک‌خطا باید فقط همان یک پیام ذخیره شود.");
            exception.Message.Should().Contain("Validation failed:", "باید پیام استاندارد شامل عبارت Validation failed باشد.");
            exception.Message.Should().Contain(errorText, "پیام باید شامل خطای ارسال‌شده باشد.");
        }

        // ======================================================================================================
        // 🔹 تست ۳: کنترل نوع Property Errors (IReadOnlyList)
        // ------------------------------------------------------------------------------------------------------
        // بررسی کنیم که پراپرتی Errors از نوع IReadOnlyList برقرار باشد و قابل تغییر نباشد.
        // ======================================================================================================
        [Fact]
        public void Errors_Property_Should_Be_ReadOnlyList()
        {
            var exception = new RiraValidationException(new List<string> { "خطا تستی" });

            // ✅ بررسی نوع پراپرتی
            exception.Errors.Should().BeAssignableTo<IReadOnlyList<string>>("طبق طراحی کلاس باید از نوع IReadOnlyList باشد.");
        }

        // ======================================================================================================
        // 🔹 تست ۴: تطبیق دقیق Message تولید‌شده با شکل قابل انتظار
        // ------------------------------------------------------------------------------------------------------
        // بررسی کنیم Message دقیقاً طبق الگوی GenerateMessage ساخته شود.
        // ======================================================================================================
        [Fact]
        public void Generated_Message_Should_Match_Expected_Format()
        {
            var errors = new List<string> { "خطا۱", "خطا۲" };
            var expectedMessage = "Validation failed: خطا۱ | خطا۲";

            var exception = new RiraValidationException(errors);

            exception.Message.Should().Be(expectedMessage, "پیام باید با الگوی ثابت کلاس تولید شود.");
        }

        // ======================================================================================================
        // 🔹 تست ۵: خالی نبودن Message و Errors
        // ------------------------------------------------------------------------------------------------------
        // بررسی کنیم که این Exception هیچ‌گاه Message یا Errors خالی برنگرداند.
        // ======================================================================================================
        [Fact]
        public void Exception_Should_Always_Have_NonEmpty_Message_And_Errors()
        {
            var exception = new RiraValidationException(new List<string> { "نمونه خطا" });

            exception.Message.Should().NotBeNullOrWhiteSpace("پیام نباید خالی باشد.");
            exception.Errors.Should().NotBeNull("لیست خطاها نباید null باشد.");
            exception.Errors.Should().Contain("نمونه خطا");
        }

        // ======================================================================================================
        // 🔹 تست ۶: سازنده با لیست خالی (رفتار مرزی)
        // ------------------------------------------------------------------------------------------------------
        // بررسی کنیم اگر لیست خالی ارسال شود، پیام ساخته‌شده باز هم ساختار معتبر داشته باشد.
        // ======================================================================================================
        [Fact]
        public void Constructor_Should_Handle_EmptyErrorsList()
        {
            var exception = new RiraValidationException(new List<string>());

            exception.Errors.Should().BeEmpty("در حالت خطاهای خالی نباید مشکلی ایجاد کند.");
            exception.Message.Should().Contain("Validation failed:", "فرمت پیام باید همچنان حفظ شود.");
        }
    }
}
