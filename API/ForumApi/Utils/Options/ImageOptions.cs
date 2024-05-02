using System.ComponentModel.DataAnnotations;

namespace ForumApi.Options
{
    public class ImageOptions
    {
        public const string Image = "Image";

        [Required]
        public string Folder { get;set; } = "";
        [Required]
        public string AvatarFolder { get;set; } = "";
        [Required]
        public string PostImageFolder { get; set; } = "";
        [Required]
        public string AvatarDefault { get; set; } = "";
        [Required]
        public int ResizeWidth { get; set; } = 0;
        [Required]
        public int ResizeHeight { get; set; } = 0;
        [Required]
        public int ResizePostWidth { get; set; } = 0;
        [Required]
        public int ResizePostHeight { get; set; } = 0;
        [Required]
        public int ImageMaxSize { get; set; } = 0;
        [Required]
        public int LimitPerAccount { get; set; } = 0;
        [Required]
        public string FileType { get; set; } = "";
        [Required]
        public int GarbageFileDeleteDelay { get ; set; } = 0;
    }
}