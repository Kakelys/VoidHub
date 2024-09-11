using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Data.Repository.Implements;

public class TokenRepository(ForumDbContext context) : RepositoryBase<Token>(context), ITokenRepository
{
    public IQueryable<Token> FindByToken(string refreshToken, bool asTracking = false) =>
        FindByCondition(t => t.RefreshToken == refreshToken, asTracking);

    public IQueryable<Token> FindByTokenWithAccount(string refreshToken, bool asTracking = false) =>
        FindByToken(refreshToken, asTracking).Include(t => t.Account);
}