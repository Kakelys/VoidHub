namespace ForumApi.Services.Utils.Interfaces
{
    public interface ISessionStorage
    {
        void Add(string context, int id);
        void Remove(string context);
        void Remove(int id);
        List<string> Get(int id);
    }
}