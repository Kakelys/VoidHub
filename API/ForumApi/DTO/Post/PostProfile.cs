using AutoMapper;
using ForumApi.Data.Models;

namespace ForumApi.DTO.DPost
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<PostEditDto, Post>();
            CreateMap<Post, PostDto>();
            CreateMap<Post, LastPost>();

            CreateMap<Post, PostResponse>();
        }        
    }
}