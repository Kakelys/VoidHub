using ForumApi.DTO.Auth;
using ForumApi.DTO.Utils;
using ForumApi.Services.Utils.Interfaces;

namespace ForumApi.Services.Utils;

public class SessionStorage : ISessionStorage
{
    private readonly Dictionary<int, ConnectedUser> _idToContextId = [];
    private readonly Dictionary<string, int> _contextIdToId = [];
    private readonly HashSet<string> _anonimContexts = [];

    public IEnumerable<string> AnonymousContexts => _anonimContexts.ToList();
    public IEnumerable<User> Users => _idToContextId.Values.Where(u => u.ConnectionIds.Count > 0).Select(l => l.User);

    public void Add(User user, string connectionId, int id)
    {
        _contextIdToId.TryAdd(connectionId, id);
        if (_idToContextId.TryGetValue(id, out ConnectedUser value))
        {
            value.ConnectionIds.Add(connectionId);
        }
        else
        {
            _idToContextId.Add(id, new ConnectedUser() { User = user, ConnectionIds = [connectionId] });
        }
    }

    public ConnectedUser Get(int id)
    {
        if (!_idToContextId.TryGetValue(id, out ConnectedUser value))
            return null;

        return value;
    }

    public int Get(string contextId)
    {
        if (_contextIdToId.TryGetValue(contextId, out var value))
            return value;

        return -1;
    }

    public ConnectedUser Remove(string contextId)
    {
        if (!_contextIdToId.TryGetValue(contextId, out int value))
            return null;

        //remove context
        var id = value;
        _contextIdToId.Remove(contextId);

        // remove from list of context
        var connectionId = _idToContextId[id].ConnectionIds.Find(c => c == contextId);
        if (connectionId == null)
            return null;

        _idToContextId[id].ConnectionIds.Remove(connectionId);

        return _idToContextId[id];
    }

    public void Remove(int id)
    {
        if (!_idToContextId.TryGetValue(id, out ConnectedUser value))
            return;

        var ctxs = value.ConnectionIds;
        _idToContextId.Remove(id);
        foreach (var ctx in ctxs)
        {
            _contextIdToId.Remove(ctx);
        }
    }

    public void AddAnonymous(string context)
    {
        _anonimContexts.Add(context);
    }

    public void RemoveAnonymous(string context)
    {
        _anonimContexts.Remove(context);
    }
}