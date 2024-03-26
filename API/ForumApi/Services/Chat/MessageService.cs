using ForumApi.Data.Models;
using ForumApi.Data.Repository.Extensions;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.Page;
using ForumApi.Services.ChatS.Interfaces;
using ForumApi.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Services.ChatS
{
    public class MessageService(IRepositoryManager rep) : IMessageService
    {
        public async Task<List<ChatMessage>> GetMesages(int chatId, Offset offset, DateTime time)
        {
            var chat = await rep.Chat.Value
                .FindByCondition(c => c.Id == chatId)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Chat not found");

            return await rep.ChatMessage.Value
                .FindByCondition(c => c.ChatId == chatId && c.CreatedAt < time.ToUniversalTime())
                .OrderByDescending(c => c.CreatedAt)
                .TakeOffset(offset)
                .ToListAsync();
        }

        public async Task<ChatMessage> SendMessage(int chatId, int accountId, string message)
        {
            var chat = await rep.Chat.Value
                .FindByCondition(c => c.Id == chatId, true)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Chat not found");

            var chatMember = chat.Members
                .FirstOrDefault(m => m.AccountId == accountId)
                ?? throw new NotFoundException("Chat member not found");

            var newMesasge = new ChatMessage() 
            {
                ChatId = chatId,
                ChatMemberId = chatMember.Id,
                Content = message
            };

            chat.Messages.Add(newMesasge);
            await rep.Save();

            return newMesasge;
        }
    }
}