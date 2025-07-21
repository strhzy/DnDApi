using System.ComponentModel.DataAnnotations;

namespace DnDAPI.Models;

public class CombatParticipant
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public int Initiative { get; set; }

    public int CurrentHitPoints { get; set; }

    public int MaxHitPoints { get; set; }

    public int ArmorClass { get; set; }

    public bool IsActive { get; set; }

    public ParticipantType Type { get; set; }

    public Guid? SourceId { get; set; }
}

public enum ParticipantType
{
    Player,
    Npc,
    Enemy
}