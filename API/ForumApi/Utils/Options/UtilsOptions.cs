using System.ComponentModel.DataAnnotations;

namespace ForumApi.Utils.Options
{
    public class UtilsOptions
    {
        public const string Utils = "Utils";

        [Required]
        [Range(1000, int.MaxValue)]
        public int OnlineStatsUpdateDelay { get; set; } = 0;
    }   
}