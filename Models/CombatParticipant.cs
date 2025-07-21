using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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

    [Required]
    public ParticipantType Type { get; set; }

    // Ссылки на разные типы сущностей (в зависимости от Type)
    public Guid? PlayerCharacterId { get; set; }
    public Guid? NpcId { get; set; }
    public Guid? EnemyId { get; set; }

    // Навигационные свойства
    [ForeignKey("PlayerCharacterId")]
    public PlayerCharacter? PlayerCharacter { get; set; }

    [ForeignKey("NpcId")]
    public NPC? Npc { get; set; }

    [ForeignKey("EnemyId")]
    public Enemy? Enemy { get; set; }

    // Метод для получения исходной сущности
    public object? GetSourceEntity()
    {
        return Type switch
        {
            ParticipantType.Player => PlayerCharacter,
            ParticipantType.Npc => Npc,
            ParticipantType.Enemy => Enemy,
            _ => null
        };
    }

    // Метод для синхронизации параметров
    public void SyncWithSource()
    {
        var source = GetSourceEntity();
        switch (source)
        {
            case PlayerCharacter pc:
                CurrentHitPoints = pc.CurrentHitPoints;
                MaxHitPoints = pc.MaxHitPoints;
                ArmorClass = pc.ArmorClass;
                break;
            case NPC npc:
                CurrentHitPoints = npc.CurrentHitPoints;
                MaxHitPoints = npc.CurrentHitPoints;
                ArmorClass = npc.ArmorClass;
                break;
            case Enemy enemy:
                CurrentHitPoints = enemy.CurrentHitPoints;
                MaxHitPoints = enemy.CurrentHitPoints;
                ArmorClass = enemy.ArmorClass;
                break;
        }
    }
}

public enum ParticipantType
{
    Player,
    Npc,
    Enemy
}