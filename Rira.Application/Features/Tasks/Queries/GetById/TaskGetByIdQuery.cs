using MediatR;
using Rira.Application.Common;
using Rira.Application.DTOs;

namespace Rira.Application.Features.Tasks.Queries.GetById
{
    /// <summary>
    /// کوئری دریافت جزئیات یک تسک بر اساس شناسه
    /// </summary>
    public class TaskGetByIdQuery : IRequest<ResponseModel<TaskDto>>
    {
        /// <summary>
        /// شناسه یکتا تسک مورد نظر
        /// </summary>
        public int Id { get; set; }
    }
}
