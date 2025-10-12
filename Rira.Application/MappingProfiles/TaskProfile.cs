using AutoMapper;
using Rira.Application.DTOs;
using Rira.Application.Features.Tasks.Commands.Create;
using Rira.Application.Features.Tasks.Commands.Update;
using Rira.Domain.Entities;

namespace Rira.Application.MappingProfiles
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<TaskEntity, TaskDto>().ReverseMap();
            CreateMap<TaskCreateCommand, TaskEntity>().ReverseMap();
            CreateMap<TaskUpdateCommand, TaskEntity>().ReverseMap();
        }
    }
}
