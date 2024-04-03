using AutoMapper;
using ForumApi.Data.Models;

namespace ForumApi.DTO.DTopic
{
    public class TopicProfile : Profile
    {
        public TopicProfile()
        {
            CreateMap<TopicEdit, Topic>();
            CreateMap<TopicNew, Topic>();
            CreateMap<Topic, TopicEdit>();
            CreateMap<Topic, TopicDto>();
        }
    }
}