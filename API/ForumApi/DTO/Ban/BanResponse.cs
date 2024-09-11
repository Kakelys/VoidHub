using ForumApi.Data.Models;

namespace ForumApi.DTO.DBan;

public class BanResponse : BanEdit
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }

    public Account Moderator { get; set; }
    public Account Account { get; set; }
    public Account UpdatedBy { get; set; }
}