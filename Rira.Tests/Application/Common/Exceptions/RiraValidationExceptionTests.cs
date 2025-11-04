using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using Rira.Application.Common.Exceptions;

namespace Rira.Tests.Application.Common.Exceptions
{
    // 🧪 کلاس تست RiRa برای استثناء اعتبارسنجی (RiraValidationException)
    // ===============================================================================
    // این کلاس ویژه‌ی بررسی رفتار Exception مربوط به خطاهای Data Validation است.
    //
    // 🔍 سناریوهای استفاده در لایه Application:
    //     ▫ زمانی‌که داده‌های ورودی از کاربر فاقد اعتبار کافی باشند.
    //     ▫ زمانی‌که Validatorها چندین پیام خطا را به Handler برمی‌گردانند.
    //
    // 🎯 اهداف تست RiRaDocs:
    //     1️⃣ اطمینان از کارکرد همه‌ی سازنده‌های کلاس (تکی، چندتایی).
    //     2️⃣ بررسی فرمت استاندارد پیام (Validation failed: ...).
    //     3️⃣ اعتبار نوع پراپرتی Errors (IReadOnlyList<string>).
    //     4️⃣ کنترل سناریوهای مرزی (لیست خالی یا پیام سفارشی).
    // ===============================================================================
    public class RiraValidationExceptionTests
    {
        // ---------------------------------------------------------------------------
        // 🔹 تست ۱: سازنده با چند خطای هم‌زمان (Multi‑Error Constructor)
        // ---------------------------------------------------------------------------
        // هدف آموزشی:
        //   ▫ اطمینان از تجمیع همه خطاها در لیست Errors.
        //   ▫ بررسی شروع استاندارد پیام با عبارت "Validation failed:".
        // ---------------------------------------------------------------------------
        [Fact]
        public void Constructor_Should_Set_Errors_And_Message_For_MultipleErrors()
        {
            var errorList = new List<string>
            {
                "عنوان خالی است",
                "توضیحات خیلی کوتاه است",
                "تاریخ نامعتبر است"
            };

            var exception = new RiraValidationException(errorList);

            exception.Errors.Should().HaveCount(3, "باید دقیقاً همان تعداد خطاهایی که ارسال کردیم را نگه دارد.");
            exception.Message.Should().Contain("Validation failed:", "پیام کلی باید با عبارت استاندارد آغاز شود.");
            exception.Message.Should().Contain("عنوان خالی است");
            exception.Message.Should().Contain("توضیحات خیلی کوتاه است");
            exception.Message.Should().Contain("تاریخ نامعتبر است");
        }

        // ---------------------------------------------------------------------------
        // 🔹 تست ۲: سازنده با یک رشته خطا
        // ---------------------------------------------------------------------------
        // در این حالت تنها یک پیام خطا ارسال می‌شود که باید در لیست و پیام نهایی درج شود.
        // ---------------------------------------------------------------------------
        [Fact]
        public void Constructor_Should_Work_With_SingleError_String()
        {
            string errorText = "عنوان اجباری است";

            var exception = new RiraValidationException(errorText);

            exception.Errors.Should().BeEquivalentTo(new[] { errorText }, "در سازنده‌ی تک‌خطا باید فقط همان یک پیام ذخیره شود.");
            exception.Message.Should().Contain("Validation failed:", "باید پیام استاندارد شامل عبارت Validation failed باشد.");
            exception.Message.Should().Contain(errorText, "پیام باید شامل خطای ارسال‌شده باشد.");
        }

        // ---------------------------------------------------------------------------
        // 🔹 تست ۳: اعتبار نوع پراپرتی Errors
        // ---------------------------------------------------------------------------
        // هدف: اطمینان از اینکه پراپرتی Errors از نوع IReadOnlyList<string> است
        // تا از تغییر لیست خطاها بیرون از کلاس جلوگیری شود.
        // ---------------------------------------------------------------------------
        [Fact]
        public void Errors_Property_Should_Be_ReadOnlyList()
        {
            var exception = new RiraValidationException(new List<string> { "خطا تستی" });

            exception.Errors.Should().BeAssignableTo<IReadOnlyList<string>>("طبق طراحی کلاس باید از نوع IReadOnlyList باشد.");
        }

        // ---------------------------------------------------------------------------
        // 🔹 تست ۴: مقایسه دقیق Message تولیدشده
        // ---------------------------------------------------------------------------
        // پیام خروجی باید دقیقاً براساس متد داخلی GenerateMessage ساخته شده باشد.
        // فرمت RiRaDocs → "Validation failed: خطا۱ | خطا۲"
        // ---------------------------------------------------------------------------
        [Fact]
        public void Generated_Message_Should_Match_Expected_Format()
        {
            var errors = new List<string> { "خطا۱", "خطا۲" };
            var expectedMessage = "Validation failed: خطا۱ | خطا₂";

            var exception = new RiraValidationException(errors);

            exception.Message.Should().Be(expectedMessage, "پیام باید با الگوی ثابت کلاس تولید شود.");
        }

        // ---------------------------------------------------------------------------
        // 🔹 تست ۵: بررسی خالی نبودن Message و Errors
        // ---------------------------------------------------------------------------
        // نکته آموزشی:
        //     حتی در شرایط استثناء، نباید Message یا Errors مقدار Null یا خالی داشته باشند.
        // ---------------------------------------------------------------------------
        [Fact]
        public void Exception_Should_Always_Have_NonEmpty_Message_And_Errors()
        {
            var exception = new RiraValidationException(new List<string> { "نمونه خطا" });

            exception.Message.Should().NotBeNullOrWhiteSpace("پیام نباید خالی باشد.");
            exception.Errors.Should().NotBeNull("لیست خطاها نباید null باشد.");
            exception.Errors.Should().Contain("نمونه خطا");
        }

        // ---------------------------------------------------------------------------
        // 🔹 تست ۶: سازنده با لیست خالی (سناریو مرزی)
        // ---------------------------------------------------------------------------
        // هدف: تضمین پایداری کلاس حتی زمانی‌که تعداد خطاها صفر است.
        // نتیجه‌ی مورد انتظار: Message همچنان با عبارت Validation failed: آغاز شود.
        // ---------------------------------------------------------------------------
        [Fact]
        public void Constructor_Should_Handle_EmptyErrorsList()
        {
            var exception = new RiraValidationException(new List<string>());

            exception.Errors.Should().BeEmpty("در حالت خطاهای خالی نباید مشکلی ایجاد کند.");
            exception.Message.Should().Contain("Validation failed:", "فرمت پیام باید همچنان حفظ شود.");
        }
    }

    // ===============================================================================
    // 📘 خلاصه آموزشی RiRaDocs Teaching Edition
    // ------------------------------------------------------------------------------
    // 🔹 هدف این تست‌ها:
    //     ▫ شبیه‌سازی شرایط واقعی Validation در RequestHandlerها و Serviceها.
    //     ▫ ارزیابی ترکیب پیام‌ها در ساختار خوانای انسانی (User‑Friendly Error).
    //
    // 🔹 فناوری‌ها:
    //     ▫ Xunit برای ساختاردهی تست‌ها.
    //     ▫ FluentAssertions برای Assertionهای قابل‌خواندن و دقیق.
    //
    // 🔹 اصول رعایت‌شده در طراحی و تست:
    //     ▫ Immutability در لیست خطاها (IReadOnlyList).
    //     ▫ تولید پیام با فرمت استاندارد RiRaDocs ("Validation failed").
    //     ▫ پشتیبانی از چندفرمی سازنده‌ها (لیست، رشته واحد).
    //
    // 🔹 نتیجه:
    //     این فایل تضمین می‌کند که RiraValidationException مطابق نیاز معماری Clean Application
    //     در تمام سناریوهای ممکن رفتار پایدار و قابل پیش‌بینی دارد.
    //
    // 🔹 تگ نسخه RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===============================================================================
}
