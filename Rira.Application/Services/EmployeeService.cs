using Microsoft.EntityFrameworkCore;
using Rira.Application.Interfaces;

namespace Rira.Application.Services
{
    /// <summary>
    /// سرویس مدیریتی کارمندان (سطح Application)
    /// هدف: ارائه‌ی متدهای کمکی برای Handlerها و Validatorها.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly IAppDbContext _context;

        public EmployeeService(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsEmailDuplicateAsync(string email)
        {
            return await _context.Employees.AnyAsync(e => e.Email == email);
        }

        public async Task<bool> IsMobileDuplicateAsync(string mobileNumber)
        {
            return await _context.Employees.AnyAsync(e => e.MobileNumber == mobileNumber);
        }
    }
}
