using System.ComponentModel.DataAnnotations;

namespace ForumApi.Utils.Options;

public class EmailOptions
{
    public const string Email = "Email";

    [Required]
    [Range(1, 65535)]
    public int Port { get; set; } = 0;
    [Required]
    public string Host { get; set; } = null!;
    [Required]
    public string From { get; set; } = null!;
    [Required]
    public string AuthKey { get; set; } = null!;
}