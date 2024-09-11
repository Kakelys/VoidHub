namespace ForumApi.DTO.DBan;

public class BanEdit
{
    public string Username { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
}