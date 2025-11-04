using Xunit;
using FluentAssertions;
using Rira.Application.Common.Exceptions;

namespace Rira.Tests.Application.Common.Exceptions
{
    // 🧪 کلاس تست واحد RiRa برای BusinessException
    // ===============================================================================
    // هدف: بررسی کامل رفتار Exception سفارشی "BusinessException" در لایه Application.
    //
    // این Exception برای نمایش خطاهای منطقی سطح Business استفاده می‌شود، نه خطاهای فنی:
    // مثل:
    //     ▫ تلاش برای تغییر وضعیت تسک "Completed".
    //     ▫ اقدام غیرمجاز روی داده‌ی حذف‌شده.
    //
    // 🎯 محورهای تست RiRaDocs:
    //     1️⃣ بررسی سازنده‌های مختلف (پیام، پیام + ErrorCode، پیام + ErrorCode + InnerException).
    //     2️⃣ تایید مقداردهی صحیح پراپرتی‌های اختصاصی (Message, ErrorCode, InnerException).
    //     3️⃣ حفظ فرمت استاندارد پیام و عدم وجود Null یا رشته خالی.
    // ===============================================================================
    public class BusinessExceptionTests
    {
        // -------------------------------------------------------------------------------
        // 🔹 تست ۱: سازنده ساده فقط با پیام (Basic Constructor)
        // -------------------------------------------------------------------------------
        // هدف آموزشی:
        //     ▫ بررسی اینکه Message مستقیماً برابر مقدار ورودی باشد.
        //     ▫ عدم تنظیم ErrorCode یا InnerException.
        // -------------------------------------------------------------------------------
        [Fact]
        public void Constructor_Should_Set_Message_Correctly()
        {
            string message = "تغییر وضعیت تسک مجاز نیست.";

            var exception = new BusinessException(message);

            exception.Message.Should().Be(message, "پیام Exception باید دقیقاً برابر پیام ورودی باشد.");
            exception.ErrorCode.Should().BeNull("در سازنده ساده هیچ کد خطایی نباید تنظیم شود.");
            exception.InnerException.Should().BeNull("در این حالت InnerException وجود ندارد.");
        }

        // -------------------------------------------------------------------------------
        // 🔹 تست ۲: سازنده با پیام و ErrorCode سفارشی
        // -------------------------------------------------------------------------------
        // هدف: بررسی مقداردهی صحیح پراپرتی ErrorCode و فرمت پیام شامل آن.
        // -------------------------------------------------------------------------------
        [Fact]
        public void Constructor_Should_Set_Custom_ErrorCode()
        {
            var exception = new BusinessException("شناسه مشتری نامعتبر است.", "ERR-CUSTOMER-ID");

            exception.Message.Should().Contain("شناسه مشتری نامعتبر است", "پیام باید شامل متن اصلی باشد.");
            exception.ErrorCode.Should().Be("ERR-CUSTOMER-ID", "کد خطا باید برابر مقدار ورودی باشد.");
        }

        // -------------------------------------------------------------------------------
        // 🔹 تست ۳: سازنده با پیام، ErrorCode و InnerException
        // -------------------------------------------------------------------------------
        // هدف: اطمینان از اینکه InnerException به‌درستی نگهداری می‌شود.
        // -------------------------------------------------------------------------------
        [Fact]
        public void Constructor_Should_Set_InnerException_Correctly()
        {
            var inner = new InvalidOperationException("درخواست نامعتبر.");

            var businessEx = new BusinessException("عملیات غیرمجاز.", "ERR-OPERATION-NOT-ALLOWED", inner);

            businessEx.Message.Should().Contain("عملیات غیرمجاز");
            businessEx.ErrorCode.Should().Be("ERR-OPERATION-NOT-ALLOWED");
            businessEx.InnerException.Should().Be(inner, "باید استثناء داخلی را نگه دارد.");
        }

        // -------------------------------------------------------------------------------
        // 🔹 تست ۴: اطمینان از غیرخالی بودن پیام در همه حالت‌های سازنده
        // -------------------------------------------------------------------------------
        // هدف: جلوگیری از تولید Message خالی در هر فرم سازنده.
        // -------------------------------------------------------------------------------
        [Fact]
        public void Exception_Should_Always_Have_NonEmpty_Message()
        {
            var ex1 = new BusinessException("پیام تستی A");
            var ex2 = new BusinessException("پیام تستی B", "ERR-B");
            var ex3 = new BusinessException("پیام تستی C", "ERR-C", new Exception("Inner"));

            ex1.Message.Should().NotBeNullOrWhiteSpace();
            ex2.Message.Should().NotBeNullOrWhiteSpace();
            ex3.Message.Should().NotBeNullOrWhiteSpace();
        }

        // -------------------------------------------------------------------------------
        // 🔹 تست ۵: بررسی فرمت استاندارد پیام شامل ErrorCode (در صورت وجود)
        // -------------------------------------------------------------------------------
        // قالب مورد انتظار RiRaDocs:
        //     «[Message] (کد خطا: [ErrorCode])»
        // هدف: تایید اینکه کلاس BusinessException پیام نهایی را با فرمت استاندارد تولید می‌کند.
        // -------------------------------------------------------------------------------
        [Fact]
        public void Message_Should_Have_Expected_Format_When_ErrorCode_Set()
        {
            var ex = new BusinessException("خطای فرآیند پرداخت", "ERR-PAYMENT");

            string expectedMessage = "خطای فرآیند پرداخت (کد خطا: ERR-PAYMENT)";

            ex.Message.Should().Be(expectedMessage, "فرمت پیام باید شامل عبارت کد خطا باشد.");
        }

        // -------------------------------------------------------------------------------
        // 🔹 تست ۶: کنترل حالت‌های Null یا رشته‌خالی برای ErrorCode
        // -------------------------------------------------------------------------------
        // هدف: جلوگیری از درج عبارت «کد خطا» در پیام زمانی که مقدار موجود نیست.
        // -------------------------------------------------------------------------------
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Constructor_Should_Handle_Null_Or_Empty_ErrorCode(string errorCode)
        {
            var ex = new BusinessException("خطای تستی", errorCode);

            ex.Message.Should().Contain("خطای تستی", "باید پیام اصلی حفظ شود.");
            if (string.IsNullOrEmpty(errorCode))
                ex.Message.Should().NotContain("کد خطا", "نباید عبارت کد خطا اضافه شود.");
        }
    }

    // ===============================================================================
    // 📘 خلاصه آموزشی RiRaDocs Teaching Edition
    // ------------------------------------------------------------------------------
    // 🔹 سطح پوشش تست:
    //     ▫ تمامی سازنده‌های کلاس BusinessException را پوشش می‌دهد.
    //     ▫ رفتار مورد انتظار در خطاهای منطقی (Business Level) بررسی می‌شود.
    //
    // 🔹 فناوری‌های استفاده‌شده:
    //     ▫ Xunit برای ساختار تست واحد.
    //     ▫ FluentAssertions برای بررسی خوانا و گویا.
    //
    // 🔹 اصول تست رعایت‌شده:
    //     ▫ استقلال از محیط خارجی (Pure Unit Test).
    //     ▫ Validation کامل Message و ErrorCode.
    //     ▫ تضمین همخوانی با فرمت استاندارد ریرا.
    //
    // 🔹 تگ نسخه RiRaDocs:
    //     RiraDocs-v2025.11.4-Stable-Final-Fixed
    // ===============================================================================
}
