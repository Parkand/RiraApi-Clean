using MediatR;
using Rira.Application.Common;
using Rira.Application.DTOs;
using System.Collections.Generic;

namespace Rira.Application.Features.Tasks.Queries.GetAll
{
    public class TaskGetAllQuery : IRequest<ResponseModel<List<TaskDto>>> { }
}
