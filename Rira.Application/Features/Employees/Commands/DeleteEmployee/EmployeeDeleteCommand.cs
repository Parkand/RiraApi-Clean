using MediatR;
using Rira.Application.Common;
using System;

namespace Rira.Application.Features.Employees.Commands.DeleteEmployee
{
    public class EmployeeDeleteCommand : IRequest<ResponseModel<int>>
    {
        public Guid Id { get; set; }  // 🔄 اصلاح از int به Guid
    }
}
