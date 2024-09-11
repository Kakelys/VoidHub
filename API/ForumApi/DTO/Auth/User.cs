namespace ForumApi.DTO.Auth
{
    public class User
    {
        public User(int id, string role, DateTime createdAt)
        {
            this.Id = id;
            this.Role = role;
            this.CreatedAt = createdAt;

        }
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string AvatarPath { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}