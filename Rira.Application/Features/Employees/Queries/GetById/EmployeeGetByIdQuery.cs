using MediatR;
using Rira.Application.Common;
using Rira.Application.DTOs;

namespace Rira.Application.Features.Employees.Queries.GetById
{
    /// <summary>
    /// میدیات‌آر کوئری برای واکشی کارمند با شناسه مشخص
    /// </summary>
    public class EmployeeGetByIdQuery : IRequest<ResponseModel<EmployeeDTO>>
    {
        public int Id { get; set; }
    }
}
