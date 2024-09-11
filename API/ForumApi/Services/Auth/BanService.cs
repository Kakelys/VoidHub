using AutoMapper;
using ForumApi.Data.Models;
using ForumApi.Data.Repository.Extensions;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.DBan;
using ForumApi.DTO.Utils;
using ForumApi.Utils.Exceptions;
using ForumApi.Services.Auth.Interfaces;
using Microsoft.EntityFrameworkCore;
using AspNetCore.Localizer.Json.Localizer;

namespace ForumApi.Services.Auth;

public class BanService(
    IRepositoryManager rep,
    IMapper mapper,
    IJsonStringLocalizer locale) : IBanService
{
    public async Task<List<BanResponse>> GetBans(Page page)
    {
        return await rep.Ban.Value
            .FindAll()
            .OrderByDescending(b => b.CreatedAt)
            .TakePage(page)
            .Select(b => new BanResponse
            {
                Id = b.Id,
                CreatedAt = b.CreatedAt,
                Moderator = b.Moderator,
                Account = b.Account,
                UpdatedBy = b.UpdatedBy,
                Reason = b.Reason,
                ExpiresAt = b.ExpiresAt,
            }).ToListAsync();
    }

    public async Task<Ban> Create(int moderId, BanEdit ban)
    {
        var moder = await rep.Account.Value
            .FindById(moderId).FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-moder"]);

        var user = await rep.Account.Value
            .FindByUsername(ban.Username).FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-user"]);

        if (moder.Id == user.Id)
            throw new BadRequestException(locale["errors.self-ban"]);

        if (moder.Role == Role.Moder && user.Role != Role.User)
            throw new ForbiddenException(locale["errors.no-permission"]);

        var banEntity = mapper.Map<Ban>(ban);
        banEntity.AccountId = user.Id;
        banEntity.ModeratorId = moderId;
        banEntity.UpdatedById = moderId;

        rep.Ban.Value.Create(banEntity);

        await rep.Save();

        return banEntity;
    }

    public async Task<Ban> Update(int moderId, int banId, BanEdit ban)
    {
        var moder = await rep.Account.Value
            .FindById(moderId)
            .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-moder"]);

        var banEntity = await rep.Ban.Value
            .FindByCondition(b => b.Id == banId, true)
            .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-ban"]);

        if (moder.Role == Role.Moder && banEntity.Account.Role != Role.User)
            throw new ForbiddenException(locale["errors.no-permission"]);

        mapper.Map(ban, banEntity);
        banEntity.UpdatedById = moderId;
        banEntity.UpdatedAt = DateTime.UtcNow;

        await rep.Save();

        return banEntity;
    }

    public async Task Delete(int moderId, string username)
    {
        var moder = await rep.Account.Value
            .FindById(moderId)
            .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-moder"]);

        var user = await rep.Account.Value
            .FindByUsername(username)
            .FirstOrDefaultAsync() ?? throw new NotFoundException(locale["errors.no-user"]);

        var activeBans = await rep.Ban.Value
            .FindByCondition(b => b.AccountId == user.Id && b.IsActive && b.ExpiresAt > DateTime.UtcNow, true)
            .ToListAsync();

        if (activeBans.Count == 0)
            throw new NotFoundException(locale["errors.no-active-ban"]);

        if (moder.Role == Role.Moder && activeBans[0].Account.Role != Role.User)
            throw new ForbiddenException(locale["errors.no-permission"]);

        rep.Ban.Value.DeleteMany(activeBans);
        foreach (var ban in activeBans)
        {
            ban.ModeratorId = moderId;
            ban.UpdatedAt = DateTime.UtcNow;
        }

        await rep.Save();
    }
}