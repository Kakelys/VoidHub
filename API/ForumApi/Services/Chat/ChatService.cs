using System.Linq.Dynamic.Core;
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
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Services.ChatS
{
    public class ChatService(
        IRepositoryManager rep, 
        IMapper mapper,
        IJsonStringLocalizer locale) : IChatService
    {
        public async Task<(ChatDto, MessageDto)> CreatePersonal(int senderId, int targetId, string message)
        {
            _ = await rep.Account.Value
                .FindById(senderId).FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-sender"]);
            _ = await rep.Account.Value
                .FindById(targetId).FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-user"]);

            // check for doubles
            var doublesCondition = rep.Chat.Value.FindByCondition(c => !c.IsGroup && c.Members.Count(m => m.AccountId == senderId || m.AccountId == targetId) == 2);
            if(senderId == targetId && doublesCondition.Any() ||
               senderId != targetId && doublesCondition.Where(c => c.Members.GroupBy(m => m.AccountId).Count() == 2).Any())
                throw new BadRequestException(locale["errors.chat-exist"]);

            var chat = new Chat();
            ChatMessage firstMessage;
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

                firstMessage = new ChatMessage()
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

            return (mapper.Map<ChatDto>(chat), mapper.Map<MessageDto>(firstMessage));
        }

        public async Task<List<ChatResponse>> Get(int accountId, Offset offset, DateTime time)
        {
            _ = rep.Account.Value
                .FindById(accountId, true)
                .FirstOrDefault() ?? throw new NotFoundException(locale["errors.no-user"]);

            var tmp = await rep.Chat.Value
                .FindByCondition(c => c.Members.Any(m => m.AccountId == accountId))
                .Select(c => new {
                    Chat = c,
                    LastMessage = c.Messages
                    .Where(m => m.CreatedAt < time.ToUniversalTime())
                    .OrderByDescending(c => c.CreatedAt)
                    .First()
                })
                .OrderByDescending(c => c.LastMessage.CreatedAt)
                .TakeOffset(offset)
                .Select(c => new ChatResponse {
                    Chat = mapper.Map<ChatDto>(c.Chat),
                    LastMessage = mapper.Map<MessageDto>(c.LastMessage),
                    Sender = mapper.Map<User>(c.LastMessage.Member.Account),
                    AnotherUser = c.Chat.IsGroup ? default : mapper.Map<User>(c.Chat.Members.First(m => m.AccountId != accountId).Account)
                })
                .ToListAsync();

            return tmp;
        }

        public async Task<ChatDto?> Get(int accountId, int targetId)
        {
            Console.WriteLine($"{accountId} {targetId}");
            var predicate = PredicateBuilder.New<Chat>(c => !c.IsGroup);
            if(accountId == targetId)
                predicate.And(c => c.Members.Count(m => m.AccountId == accountId) == 2);
            else
                predicate.And(c => c.Members.Where(m => m.AccountId == accountId || m.AccountId == targetId).GroupBy(m => m.AccountId).Count() == 2);

            var chat = await rep.Chat.Value
                .FindByCondition(predicate, true)
                .FirstOrDefaultAsync();

            return chat == null ? null : mapper.Map<ChatDto>(chat);
        }

        public async Task<ChatInfo?> Get(int chatId)
        {
            return await rep.Chat.Value
                .FindByCondition(c => c.Id == chatId, true)
                .Select(c => new ChatInfo {
                    Chat = mapper.Map<ChatDto>(c),
                    Members = mapper.Map<List<User>>(c.Members.Select(c => c.Account).ToList())
                })
                .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-chat"]);
        }
    }
}