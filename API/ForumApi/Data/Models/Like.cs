namespace ForumApi.Data.Models;

public class Like
{
    public int AccountId { get; set; }
    public int PostId { get; set; }

    public virtual Account Account { get; set; }
    public virtual Post Post { get; set; }
}