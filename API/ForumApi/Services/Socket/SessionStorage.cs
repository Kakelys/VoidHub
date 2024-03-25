using ForumApi.Services.Socket.Interfaces;

namespace ForumApi.Services.Socket
{
    public class SessionStorage : ISessionStorage
    {
        private Dictionary<int, List<string>> _idToContextId = new();
        private Dictionary<string, int> _contextIdToId = new();

        public void Add(string context, int id)
        {
            _contextIdToId.TryAdd(context, id);
            _idToContextId.TryAdd(id, [context]);

            Console.WriteLine(_idToContextId.Count());
        }

        public List<string> Get(int id)
        {
            if(!_idToContextId.ContainsKey(id))
                return [];

            return _idToContextId[id];
        }

        public void Remove(string context)
        {
            if(!_contextIdToId.ContainsKey(context))
                return;

            //remove context
            var id = _contextIdToId[context];
            _contextIdToId.Remove(context);

            // remove from list of context
            var ctx = _idToContextId[id].Where(c => c == context).FirstOrDefault();
            if(ctx == null)
                return;

            _idToContextId[id].Remove(ctx);
        }

        public void Remove(int id)
        {
            if(!_idToContextId.ContainsKey(id))
                return;

            var ctxs = _idToContextId[id];
            _idToContextId.Remove(id);
            foreach(var ctx in ctxs)
            {
                _contextIdToId.Remove(ctx);
            }
        }
    }
}