using System.ComponentModel.DataAnnotations;

namespace DnDAPI.Models;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.Player;

    public List<PlayerCharacter> Characters { get; set; } = new();
    public List<Campaign> Campaigns { get; set; } = new();
}

public enum UserRole
{
    Master,
    Player
}