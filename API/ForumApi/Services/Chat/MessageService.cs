using AspNetCore.Localizer.Json.Localizer;
using AutoMapper;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Extensions;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.Auth;
using ForumApi.DTO.DChat;
using ForumApi.DTO.Utils;
using ForumApi.Services.ChatS.Interfaces;
using ForumApi.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Services.ChatS;

public class MessageService(
    IRepositoryManager rep,
    IMapper mapper,
    IJsonStringLocalizer locale) : IMessageService
{
    public async Task<List<MessageResponse>> GetMessages(int chatId, Offset offset, DateTime time)
    {
        _ = await rep.Chat.Value
            .FindByCondition(c => c.Id == chatId)
            .FirstOrDefaultAsync()
                ?? throw new NotFoundException(locale["errors.no-chat"]);

        return await rep.ChatMessage.Value
            .FindByCondition(c => c.ChatId == chatId && c.CreatedAt < time.ToUniversalTime())
            .OrderByDescending(c => c.CreatedAt)
            .Select(m => new MessageResponse
            {
                Sender = mapper.Map<User>(m.Member.Account),
                Message = mapper.Map<MessageDto>(m)
            })
            .TakeOffset(offset)
            .ToListAsync();
    }

    public async Task<MessageResponse> SendMessage(int chatId, int accountId, string message)
    {
        var chat = await rep.Chat.Value
            .FindByCondition(c => c.Id == chatId, true)
            .FirstOrDefaultAsync()
                ?? throw new NotFoundException(locale["errors.no-chat"]);

        var chatMember = chat.Members
            .Find(m => m.AccountId == accountId)
                ?? throw new NotFoundException(locale["errors.no-chat-member"]);

        var newMessage = new ChatMessage()
        {
            ChatId = chatId,
            ChatMemberId = chatMember.Id,
            Content = message
        };

        chat.Messages.Add(newMessage);
        await rep.Save();

        return new MessageResponse
        {
            Message = mapper.Map<MessageDto>(newMessage),
            Sender = mapper.Map<User>(chatMember.Account),
        };
    }
}