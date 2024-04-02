using AutoMapper;
using ForumApi.Data.Models;

namespace ForumApi.DTO.DChat
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<Chat, ChatDto>();
            CreateMap<ChatMessage, MessageDto>();

            CreateMap<Message, MessageDto>();
        }
    }
}