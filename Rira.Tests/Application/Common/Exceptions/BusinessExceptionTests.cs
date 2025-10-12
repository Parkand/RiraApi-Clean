using Xunit;
using FluentAssertions;
using Rira.Application.Common.Exceptions;

namespace Rira.Tests.Application.Common.Exceptions
{
    /// <summary>
    /// ✅ تست واحد کامل برای کلاس BusinessException
    /// -------------------------------------------------------------
    /// این Exception در موقع بروز خطاهای منطقی در سطح کسب‌و‌کار (دومین لایه)
    /// استفاده می‌شود. مثلاً:
    ///   - اقدام غیرمجاز (Update روی تسک بسته‌شده)
    ///   - نقض قوانین Validation تجاری (تغییر وضعیت اشتباه)
    ///
    /// هدف تست‌ها:
    ///   ▫ بررسی سازنده‌ها (پیام ساده، پیام + کد محدودیت)
    ///   ▫ بررسی فرمت صحیح پیام نهایی
    ///   ▫ اطمینان از مقداردهی درست پراپرتی‌های Custom
    /// </summary>
    public class BusinessExceptionTests
    {
        // ======================================================================================================
        // 🔹 تست ۱: سازنده ساده فقط با پیام
        // ------------------------------------------------------------------------------------------------------
        // بررسی کنیم که Exception به‌درستی پیام را تنظیم کند.
        // ======================================================================================================
        [Fact]
        public void Constructor_Should_Set_Message_Correctly()
        {
            // 🧩 داده تست
            string message = "تغییر وضعیت تسک مجاز نیست.";

            // 🧪 اجرا
            var exception = new BusinessException(message);

            // ✅ بررسی‌ها
            exception.Message.Should().Be(message, "پیام Exception باید دقیقاً برابر پیام ورودی باشد.");
            exception.ErrorCode.Should().BeNull("در سازنده ساده هیچ کد خطایی نباید تنظیم شود.");
            exception.InnerException.Should().BeNull("در این حالت InnerException وجود ندارد.");
        }

        // ======================================================================================================
        // 🔹 تست ۲: سازنده با پیام و کد خطای سفارشی
        // ------------------------------------------------------------------------------------------------------
        // بررسی کنیم پراپرتی ErrorCode به‌درستی مقداردهی شود.
        // ======================================================================================================
        [Fact]
        public void Constructor_Should_Set_Custom_ErrorCode()
        {
            // 🧩 داده تست
            var exception = new BusinessException("شناسه مشتری نامعتبر است.", "ERR-CUSTOMER-ID");

            // ✅ بررسی‌ها
            exception.Message.Should().Contain("شناسه مشتری نامعتبر است", "پیام باید شامل متن اصلی باشد.");
            exception.ErrorCode.Should().Be("ERR-CUSTOMER-ID", "کد خطا باید برابر مقدار ورودی باشد.");
        }

        // ======================================================================================================
        // 🔹 تست ۳: سازنده با پیام، کد خطا و InnerException
        // ------------------------------------------------------------------------------------------------------
        // بررسی کنیم که Exception داخلی به‌درستی تنظیم شود.
        // ======================================================================================================
        [Fact]
        public void Constructor_Should_Set_InnerException_Correctly()
        {
            // 🧩 ساخت Exception داخلی
            var inner = new InvalidOperationException("درخواست نامعتبر.");

            // 🧪 ساخت BusinessException
            var businessEx = new BusinessException("عملیات غیرمجاز.", "ERR-OPERATION-NOT-ALLOWED", inner);

            // ✅ بررسی‌ها
            businessEx.Message.Should().Contain("عملیات غیرمجاز");
            businessEx.ErrorCode.Should().Be("ERR-OPERATION-NOT-ALLOWED");
            businessEx.InnerException.Should().Be(inner, "باید استثناء داخلی را نگه دارد.");
        }

        // ======================================================================================================
        // 🔹 تست ۴: عدم خالی بودن پیام در تمام حالات سازنده
        // ------------------------------------------------------------------------------------------------------
        // بررسی اطمینان از عدم تولید پیام خالی توسط هیچ سازنده.
        // ======================================================================================================
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

        // ======================================================================================================
        // 🔹 تست ۵: بررسی فرمت استاندارد پیام شامل ErrorCode (در صورت وجود)
        // ------------------------------------------------------------------------------------------------------
        // پیام نهایی باید شامل ErrorCode بعد از متن اصلی باشد اگر مقداردهی شده باشد.
        // ======================================================================================================
        [Fact]
        public void Message_Should_Have_Expected_Format_When_ErrorCode_Set()
        {
            // 🧩 ساخت نمونه
            var ex = new BusinessException("خطای فرآیند پرداخت", "ERR-PAYMENT");

            // 🧩 انتظار خروجی
            string expectedMessage =
                "خطای فرآیند پرداخت (کد خطا: ERR-PAYMENT)";

            ex.Message.Should().Be(expectedMessage, "فرمت پیام باید شامل عبارت کد خطا باشد.");
        }

        // ======================================================================================================
        // 🔹 تست ۶: بررسی حالت نادر - کد خطا خالی یا Null
        // ------------------------------------------------------------------------------------------------------
        // حتی در حالت Null یا رشته خالی، نباید ساختار پیام را بشکند.
        // ======================================================================================================
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
}
