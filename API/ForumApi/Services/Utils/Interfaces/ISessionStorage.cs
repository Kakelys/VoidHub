using ForumApi.DTO.Auth;
using ForumApi.DTO.Utils;

namespace ForumApi.Services.Utils.Interfaces
{
    public interface ISessionStorage
    {
        void Add(User user, string connectionId, int id);
        void AddAnonymous(string contextId);
        ConnectedUser? Remove(string contextId);
        void Remove(int id);
        void RemoveAnonymous(string contextId);
        ConnectedUser? Get(int id);
        int Get(string contextId);

        IEnumerable<string> AnonymousContexts { get; }
        IEnumerable<User> Users { get; }
    }
}