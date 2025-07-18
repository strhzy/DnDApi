using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    
    [ForeignKey("PlayerCharacter")]
    public Guid? PlayerCharacterId { get; set; }
    
    [ForeignKey("Enemy")]
    public Guid? EnemyId { get; set; }

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    public int? AttackBonus { get; set; }

    [StringLength(50)]
    public string DamageDice { get; set; } = string.Empty;
}