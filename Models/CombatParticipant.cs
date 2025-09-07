using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DnDAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace DnDAPI.Models;

public class CombatParticipant
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public int Initiative { get; set; }

    public int CurrentHitPoints { get; set; }

    public int MaxHitPoints { get; set; }

    public int ArmorClass { get; set; }

    public bool IsActive { get; set; }
    
    [Required]
    public ParticipantType Type { get; set; }

    public Guid? SourceId { get; set; }

    [ForeignKey("Combat")]
    public Guid CombatId { get; set; }

    [JsonIgnore]
    public Combat? Combat { get; set; }

    // Метод для получения исходной сущности
    public object? GetSourceEntity()
    {
        return Type switch
        {
            ParticipantType.Player => ApiHelper.Get<PlayerCharacter>("PlayerCharacter", SourceId ?? Guid.Empty),
            ParticipantType.Npc => ApiHelper.Get<NPC>("NPC", SourceId ?? Guid.Empty),
            ParticipantType.Enemy => ApiHelper.Get<Enemy>("Enemy", SourceId ?? Guid.Empty),
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