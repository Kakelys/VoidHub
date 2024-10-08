namespace ForumApi.Data.Models;

public class Token
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }

    public virtual Account Account { get; set; }
}