using MediatR;
using Rira.Application.Common;

namespace Rira.Application.Features.Employees.Commands.DeleteEmployee
{
    /// <summary>
    /// 🗑 فرمان حذف کارمند از سیستم.
    /// شامل فقط شناسه کارمند برای حذف است.
    /// </summary>
    public class EmployeeDeleteCommand : IRequest<ResponseModel<int>>
    {
        public int Id { get; set; }
    }
}
