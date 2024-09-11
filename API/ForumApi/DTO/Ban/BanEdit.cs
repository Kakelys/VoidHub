namespace ForumApi.DTO.DBan;

public class BanEdit
{
    public string Username { get; set; }
    public string Reason { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
}