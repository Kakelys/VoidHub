using System.Linq.Dynamic.Core;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Extensions;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.Page;
using ForumApi.Services.ChatS.Interfaces;
using ForumApi.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Services.ChatS
{
    public class ChatService(IRepositoryManager rep) : IChatService
    {
        public async Task<Chat> CreatePersonal(int senderId, int targetId, string message)
        {
            var sender = await rep.Account.Value
                .FindById(senderId).FirstOrDefaultAsync() ?? throw new NotFoundException("Sender not found");
            var target = await rep.Account.Value
                .FindById(targetId).FirstOrDefaultAsync() ?? throw new NotFoundException("Account not found");

            // check for doubles
            if(rep.Chat.Value
                .FindByCondition(c => !c.IsGroup && c.Members.Count(m => m.AccountId == senderId || m.AccountId == targetId) == 2)
                .Any()
            )
                throw new BadRequestException("Chat alredy exist");

            var chat = new Chat();
            var chatMember1 = new ChatMember() 
            {
                ChatId = chat.Id,
                AccountId = senderId
            };

            var chatMember2 = new ChatMember() 
            {
                ChatId = chat.Id,
                AccountId = targetId
            };

            await rep.BeginTransaction();
            try
            {
                rep.Chat.Value.Create(chat);

                chat.Members.Add(chatMember1);
                chat.Members.Add(chatMember2);

                await rep.Save();

                var firstMessage = new ChatMessage()
                {
                    ChatId = chat.Id,
                    ChatMemberId = chatMember1.Id,
                    Content = message
                };

                chat.Messages.Add(firstMessage);

                await rep.Save();
                await rep.Commit();
            }   
            catch
            {
                await rep.Rollback();

                throw;
            }

            return chat;
        }

        public async Task<List<Chat>> Get(int accountId, Offset offset, DateTime time)
        {
            var acc = rep.Account.Value
                .FindById(accountId, true)
                .FirstOrDefault() ?? throw new NotFoundException("Account not found");

            var chats = await rep.Chat.Value
                .FindByCondition(c => c.Members.Where(m => m.AccountId == accountId).Any())
                .OrderByDescending(c => 
                    c.Messages
                    .Where(m => m.CreatedAt < time.ToUniversalTime())
                    .Max(m => m.CreatedAt)
                )
                .TakeOffset(offset)
                .ToListAsync();

            return chats;
        }
    }
}