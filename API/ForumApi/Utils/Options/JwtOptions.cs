using System.ComponentModel.DataAnnotations;

namespace ForumApi.Options;

public class JwtOptions
{
    public const string Jwt = "Jwt";

    [Required]
    public string Issuer { get; set; } = "";
    [Required]
    public string Audience { get; set; } = "";
    [Required]
    public string AccessSecret { get; set; } = "";
    [Required]
    public string RefreshSecret { get; set; } = "";
    [Required]
    public string ConfirmSecret { get; set; } = "";
    [Required]
    public string RecoverSecret { get; set; } = "";
    [Required]
    [Range(0, int.MaxValue)]
    public int AccessLifetimeInMinutes { get; set; } = 0;
    [Required]
    [Range(0, int.MaxValue)]
    public int RefreshLifetimeInMinutes { get; set; } = 0;
    [Required]
    [Range(0, int.MaxValue)]
    public int ConfirmLifetimeInMinutes { get; set; } = 0;
    [Required]
    [Range(0, int.MaxValue)]
    public int RecoverLifetimeInMinutes { get; set; } = 0;
    [Range(0, int.MaxValue)]
    public int MaxTokenCount { get; set; } = 0;
}