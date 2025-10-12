using System.Threading.Tasks;
using Rira.Application.DTOs;

namespace Rira.Application.Interfaces
{
    /// <summary>
    /// اینترفیس سرویس مربوط به کارمندان.
    /// حاوی متدهای سطح میانی (نه CRUD اصلی، چون آن‌ها در Handlerها هستند).
    /// </summary>
    public interface IEmployeeService
    {
        /// بررسی تکراری بودن ایمیل
        Task<bool> IsEmailDuplicateAsync(string email);

        /// بررسی تکراری بودن موبایل
        Task<bool> IsMobileDuplicateAsync(string mobileNumber);
    }
}
