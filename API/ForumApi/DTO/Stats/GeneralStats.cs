using ForumApi.DTO.Auth;
using ForumApi.Migrations;

namespace ForumApi.DTO.Stats
{
    public class GeneralStats
    {
        public int TopicCount { get; set; }
        public int PostCount { get; set; }
        public int UserCount { get; set; }
        public User? LastUser { get; set; }
    }
}