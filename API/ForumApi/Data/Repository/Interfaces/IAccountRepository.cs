using ForumApi.Data.Models;

namespace ForumApi.Data.Repository.Interfaces;

public interface IAccountRepository : IRepositoryBase<Account>
{
    IQueryable<Account> FindById(int id, bool asTracking = false);
    /// <summary>
    /// Searching with ignoring case
    /// </summary>
    IQueryable<Account> FindByEmail(string email, bool asTracking = false);
    /// <summary>
    /// Searching with ignoring case
    /// </summary>
    IQueryable<Account> FindByUsername(string username, bool asTracking = false);
    /// <summary>
    /// Searching with ignoring case
    /// </summary>
    IQueryable<Account> FindByLogin(string login, bool asTracking = false);
    IQueryable<Account> FindByLoginWithTokens(string login, bool asTracking = false);
    IQueryable<Account> FindByLoginOrEmail(string loginOrEmail, bool asTracking = false);
    /// <summary>
    /// Set DeletedAt time to current time and change email to email-deleted
    /// <para>Not saving db changes</para>
    /// </summary>
    new void Delete(Account entity);
}