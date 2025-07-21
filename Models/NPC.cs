using System.ComponentModel.DataAnnotations;

namespace DnDAPI.Models;

public class NPC
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(50)]
    public string Race { get; set; } = string.Empty;

    [StringLength(100)]
    public string Occupation { get; set; } = string.Empty;

    public int CurrentHitPoints { get; set; } = 10;

    public int ArmorClass { get; set; } = 10;

    [StringLength(500)]
    public string PersonalityTraits { get; set; } = string.Empty;

    [StringLength(500)]
    public string Ideals { get; set; } = string.Empty;

    [StringLength(500)]
    public string Bonds { get; set; } = string.Empty;

    [StringLength(500)]
    public string Flaws { get; set; } = string.Empty;
}