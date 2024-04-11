using ForumApi.Services.Utils.Interfaces;

namespace ForumApi.Services.Utils
{
    public class SessionStorage : ISessionStorage
    {
        private readonly Dictionary<int, List<string>> _idToContextId = [];
        private readonly Dictionary<string, int> _contextIdToId = [];

        public void Add(string context, int id)
        {
            _contextIdToId.TryAdd(context, id);
            if(_idToContextId.TryGetValue(id, out List<string>? value))
                value.Add(context);
            else
                _idToContextId.Add(id, [context]);
        }

        public List<string> Get(int id)
        {
            if(!_idToContextId.TryGetValue(id, out List<string>? value))
                return [];

            return value;
        }

        public void Remove(string context)
        {
            if(!_contextIdToId.TryGetValue(context, out int value))
                return;

            //remove context
            var id = value;
            _contextIdToId.Remove(context);

            // remove from list of context
            var ctx = _idToContextId[id].Find(c => c == context);
            if(ctx == null)
                return;

            _idToContextId[id].Remove(ctx);
        }

        public void Remove(int id)
        {
            if(!_idToContextId.TryGetValue(id, out List<string>? value))
                return;

            var ctxs = value;
            _idToContextId.Remove(id);
            foreach(var ctx in ctxs)
            {
                _contextIdToId.Remove(ctx);
            }
        }
    }
}