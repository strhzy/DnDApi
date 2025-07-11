using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DnDAPI.Models;

public class Attack
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    [JsonPropertyName("desc")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("attack_bonus")]
    public int? AttackBonus { get; set; }

    [StringLength(50)]
    [JsonPropertyName("damage_dice")]
    public string DamageDice { get; set; } = string.Empty;
}