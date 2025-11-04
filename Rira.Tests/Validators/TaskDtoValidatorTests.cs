// ===========================================================
// 📘 RiRaDocs Teaching Edition (Farsi Inline)
// File: ResponseModelTests.cs
// Layer: Tests → Application → Common
// Context: Unit Tests for ResponseModel<T>
// هدف: اطمینان از صحت رفتار ResponseModel در معماری Clean
// انتشار: RiraDocs-v2025.11.4-Stable-Final-Fixed
// ===========================================================

using Xunit;
using FluentAssertions;
using Rira.Application.Common;

namespace Rira.Tests.Application.Common
{
    /// <summary>
    /// 🧠 کلاس تست واحد برای ResponseModel<T>
    /// -----------------------------------------------------------
    /// 🎯 هدف آموزشی RiRaDocs:
    ///   ▫ بررسی سازنده‌ها و متدهای ایستای کلاس ResponseModel
    ///   ▫ اطمینان از سازگاری رفتار خروجی ResponseModel با معماری Clean Application
    ///   ▫ تثبیت استاندارد پیام و قالب خروجی سرویس‌ها در تست‌های واحد
    /// -----------------------------------------------------------
    /// ResponseModel یکی از اجزای کلیدی لایه Application است و
    /// هدفش استاندارد کردن خروجی تمام سرویس‌ها (Handler / Validator / Service) می‌باشد.
    /// </summary>
    public class ResponseModelTests
    {
        // ======================================================================================================
        // 🟢 تست ۱: بررسی سازنده‌ی سه‌پارامتری در حالت موفق
        // ------------------------------------------------------------------------------------------------------
        // 🎯 این تست می‌سنجد که سازنده‌ی ResponseModel<T> مقادیر Success، Message و Data را به‌درستی تنظیم کند.
        // ======================================================================================================
        [Fact]
        public void Constructor_Should_Assign_Properties_Correctly()
        {
            // 🔧 داده‌ی شبیه DTO (نمونه‌ای از داده‌ی بازگشتی موفق سرویس)
            var fakeDto = new { Id = 1, Title = "تسک تستی" };

            // 🏗️ ساخت ResponseModel موفق
            var response = ResponseModel<object>.Ok(fakeDto, "عملیات با موفقیت انجام شد");

            // ✅ بررسی مقادیر
            response.Success.Should().BeTrue("زیرا عملیات موفق اعلام شده است.");
            response.Message.Should().Be("عملیات با موفقیت انجام شد", "پیام باید عیناً مطابق ورودی باشد.");
            response.Data.Should().NotBeNull("در حالت موفق، Data باید مقداردهی شود.")
                          .And.Be(fakeDto);
        }

        // ======================================================================================================
        // 🔴 تست ۲: سازنده‌ی مشخص در حالت شکست (Failure)
        // ------------------------------------------------------------------------------------------------------
        // 🎯 هدف: اطمینان از مقداردهی صحیح Success=false و Data=null در وضعیت خطا.
        // ======================================================================================================
        [Fact]
        public void Constructor_Should_Handle_Failure_Correctly()
        {
            var response = new ResponseModel<string>(false, "خطا در پردازش داده", 500, null);

            response.Success.Should().BeFalse();
            response.Message.Should().Be("خطا در پردازش داده");
            response.Data.Should().BeNull();
        }

        // ======================================================================================================
        // 🟩 تست ۳: متد ایستا‌ی Ok()
        // ------------------------------------------------------------------------------------------------------
        // 🎯 بررسی می‌کند که متد کمکی Ok، پاسخ موفقی تولید کرده و مقادیر را درست ست کند.
        // ======================================================================================================
        [Fact]
        public void Ok_Static_Method_Should_Create_Success_Response()
        {
            var dto = new { Id = 5, Title = "تسک نمونه" };

            var response = ResponseModel<object>.Ok(dto, "تسک با موفقیت ثبت شد");

            response.Success.Should().BeTrue();
            response.Message.Should().Contain("موفقیت");
            response.Data.Should().Be(dto);
        }

        // ======================================================================================================
        // 🟥 تست ۴: متد ایستا‌ی Fail()
        // ------------------------------------------------------------------------------------------------------
        // 🎯 هدف: بررسی ساخت پاسخ خطا (Success=false ، Data=null ، Message دقیق).
        // ======================================================================================================
        [Fact]
        public void Fail_Static_Method_Should_Create_Failure_Response()
        {
            var response = ResponseModel<object>.Fail("آیتم مورد نظر یافت نشد");

            response.Success.Should().BeFalse();
            response.Data.Should().BeNull();
            response.Message.Should().Be("آیتم مورد نظر یافت نشد");
        }

        // ======================================================================================================
        // 🟣 تست ۵: بررسی رفتار ترکیبی Ok / Fail
        // ------------------------------------------------------------------------------------------------------
        // 🎯 هدف: اطمینان از عملکرد مستقل و صحیح هر دو متد در یک واحد تست.
        // ======================================================================================================
        [Fact]
        public void Ok_And_Fail_Combination_Should_Behave_Correctly()
        {
            // حالت موفق
            var successData = new { Id = 101, Title = "ثبت انجام شد" };
            var okResponse = ResponseModel<object>.Ok(successData, "عملیات با موفقیت انجام شد");
            okResponse.Success.Should().BeTrue();
            okResponse.Message.Should().Contain("موفقیت");
            okResponse.Data.Should().Be(successData);

            // حالت خطا
            var failResponse = ResponseModel<object>.Fail("خطا در دریافت داده");
            failResponse.Success.Should().BeFalse();
            failResponse.Data.Should().BeNull();
            failResponse.Message.Should().Be("خطا در دریافت داده");
        }

        // ======================================================================================================
        // ⚪ تست ۶: سازنده‌ی پیش‌فرض (Default Constructor)
        // ------------------------------------------------------------------------------------------------------
        // 🎯 سنجش مقادیر اولیه‌ای که در ایجاد بدون آرگومان انتظار می‌رود.
        // ======================================================================================================
        [Fact]
        public void Default_Constructor_Should_Have_Correct_Defaults()
        {
            var response = new ResponseModel<string>();

            response.Success.Should().BeFalse("در سازنده‌ی پیش‌فرض مقداردهی صورت نمی‌گیرد.");
            response.Message.Should().BeEmpty("Message به‌صورت پیش‌فرض رشته‌ی خالی است.");
            response.Data.Should().BeNull("هنوز هیچ داده‌ای تنظیم نشده است.");
        }
    }
}

// ===========================================================
// 📘 جمع‌بندی آموزشی (RiRaDocs Summary)
// -----------------------------------------------------------
// ▫ ResponseModel نقش کانال مشترک خروجی بین Handler و کلاینت را دارد.
// ▫ با تست‌های فوق اطمینان حاصل می‌شود که:
//     → ساخت نمونه موفق (Ok) و ناموفق (Fail) مطابق استاندارد معماری عمل می‌کند.
//     → مقادیر Success, Message, Data همیشه به‌شکل قابل پیش‌بینی مقداردهی می‌شوند.
// ▫ استفاده از xUnit + FluentAssertions باعث افزایش خوانایی و Self‑Documenting بودن کد تست‌ها می‌شود.
// ▫ تگ انتشار: RiraDocs‑v2025.11.4‑Stable‑Final‑Fixed
// ===========================================================
