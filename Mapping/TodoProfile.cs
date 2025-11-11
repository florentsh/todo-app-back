using AutoMapper;
using BackTodoApi.Dtos;
using BackTodoApi.Models;

namespace BackTodoApi.Mapping;

public class TodoProfile : Profile
{
    public TodoProfile()
    {
        CreateMap<Todo, TodoDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.UserName : null));


        CreateMap<CreateTodoDto, Todo>();

        CreateMap<UpdateTodoDto, Todo>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
