using AutoMapper;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.Auth;
using ForumApi.DTO.DAccount;
using ForumApi.DTO.DBan;
using ForumApi.Utils.Exceptions;
using ForumApi.Utils.Extensions;
using ForumApi.Options;
using ForumApi.Services.ForumS.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using AspNetCore.Localizer.Json.Localizer;
using ForumApi.Data.Models;

namespace ForumApi.Services.ForumS;

public class AccountService(
    IRepositoryManager rep,
    IMapper mapper,
    IOptions<ImageOptions> imageOptions,
    IJsonStringLocalizer locale
) : IAccountService
{
    private readonly ImageOptions _imageOptions = imageOptions.Value;

    public async Task<AccountResponse> Get(int id)
    {
        return await rep.Account.Value
            .FindByCondition(a => a.Id == id)
            .Select(a => new AccountResponse
            {
                Id = a.Id,
                Username = a.Username,
                Role = a.Role,
                AvatarPath = a.AvatarPath,
                CreatedAt = a.CreatedAt,
                PostsCount = a.Posts.Count(p => p.AccountId == a.Id && p.DeletedAt == null),
                TopicsCount = a.Topics.Count(t => t.AccountId == a.Id && t.DeletedAt == null),
                Ban = a.RecievedBans.Where(b => b.IsActive && b.ExpiresAt > DateTime.UtcNow)
                    .OrderByDescending(b => b.IsActive)
                    .ThenByDescending(b => b.ExpiresAt)
                    .Select(b => mapper.Map<BanEdit>(b))
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync()
                ?? throw new NotFoundException(locale["errors.no-user"]);
    }

    public async Task<User> GetUser(int id)
    {
        var account = await rep.Account.Value
            .FindById(id)
            .FirstOrDefaultAsync()
                ?? throw new NotFoundException(locale["errors.no-user"]);

        return mapper.Map<User>(account);
    }

    public async Task Delete(int id)
    {
        var account = await rep.Account.Value
            .FindByCondition(a => a.Id == id, true)
            .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-user"]);

        rep.Account.Value.Delete(account);
        account.Tokens.Clear();

        await rep.Save();
    }

    public async Task<AuthUser> Update(int targetId, int senderId, AccountDto accountDto)
    {
        var user = await rep.Account.Value.FindById(targetId, true)
            .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-user"]);

        if (!string.IsNullOrEmpty(accountDto.Username) && accountDto.Username != user.Username)
        {
            if (await rep.Account.Value.FindByUsername(accountDto.Username).AnyAsync())
            {
                throw new BadRequestException(locale["errors.username-exist"]);
            }

            user.Username = accountDto.Username;
        }

        if (!string.IsNullOrEmpty(accountDto.Email) && accountDto.Email != user.Email)
        {
            if (await rep.Account.Value.FindByEmail(accountDto.Email).AnyAsync())
            {
                throw new BadRequestException(locale["errors.email-exist"]);
            }

            user.Email = accountDto.Email;
            user.IsEmailConfirmed = false;
        }

        if (accountDto.Role != RoleEnum.None)
        {
            if (targetId == senderId)
            {
                throw new BadRequestException(locale["errors.self-role"]);
            }

            user.Role = accountDto.Role.ToString();
        }

        if (!string.IsNullOrEmpty(accountDto.OldPassword) && !string.IsNullOrEmpty(accountDto.NewPassword))
        {
            if (accountDto.OldPassword == accountDto.NewPassword)
            {
                throw new BadRequestException(locale["errors.same-new-password"]);
            }

            if (!PasswordHelper.Verify(accountDto.OldPassword, user.PasswordHash))
            {
                throw new BadRequestException(locale["errors.wrong-old-password"]);
            }

            user.PasswordHash = PasswordHelper.Hash(accountDto.NewPassword);
        }

        await rep.Save();

        return mapper.Map<AuthUser>(user);
    }

    public async Task<AuthUser> UpdateImg(int accountId, string newPath)
    {
        var user = await rep.Account.Value.FindById(accountId, true)
            .FirstOrDefaultAsync()
                ?? throw new NotFoundException(locale["errors.no-user"]);

        await rep.BeginTransaction();
        try
        {
            // store old file path
            var oldPath = user.AvatarPath;

            user.AvatarPath = newPath;
            await rep.Save();

            // delete if changed to default
            if (user.AvatarPath == _imageOptions.AvatarDefault
               && System.IO.File.Exists($"{_imageOptions.Folder}/{oldPath}"))
            {
                System.IO.File.Delete($"{_imageOptions.Folder}/{oldPath}");
            }

            await rep.Commit();
        }
        catch
        {
            await rep.Rollback();
            throw;
        }

        return mapper.Map<AuthUser>(user);
    }
}