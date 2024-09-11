using AutoMapper;
using ForumApi.Data.Models;

namespace ForumApi.DTO.DForum;

public class ForumProfile : Profile
{
    public ForumProfile()
    {
        CreateMap<ForumEdit, Forum>();
        CreateMap<Forum, ForumEdit>();

        CreateMap<Forum, ForumDto>();
    }
}