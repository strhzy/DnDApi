using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DnDAPI.Models;

public abstract class CombatParticipant
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

    public abstract void SyncWithSource(DnDContext context);
}

public class PlayerCombatParticipant : CombatParticipant
{
    public Guid PlayerCharacterId { get; set; }
    
    [ForeignKey("PlayerCharacterId")]
    public PlayerCharacter PlayerCharacter { get; set; }

    public override void SyncWithSource(DnDContext context)
    {
        if (PlayerCharacter == null)
            PlayerCharacter = context.PlayerCharacters.Find(PlayerCharacterId);

        if (PlayerCharacter != null)
        {
            PlayerCharacter.CurrentHitPoints = CurrentHitPoints;
            PlayerCharacter.TemporaryHitPoints = Math.Max(0, CurrentHitPoints - PlayerCharacter.MaxHitPoints);
            context.Entry(PlayerCharacter).State = EntityState.Modified;
        }
    }
}

public class NpcCombatParticipant : CombatParticipant
{
    public Guid NpcId { get; set; }
    
    [ForeignKey("NpcId")]
    public NPC Npc { get; set; }

    public override void SyncWithSource(DnDContext context)
    {
        if (Npc == null)
            Npc = context.NPCs.Find(NpcId);

        if (Npc != null)
        {
            Npc.CurrentHitPoints = CurrentHitPoints;
            context.Entry(Npc).State = EntityState.Modified;
        }
    }
}

public class EnemyCombatParticipant : CombatParticipant
{
    public Guid EnemyId { get; set; }

    [ForeignKey("EnemyId")] public Enemy Enemy { get; set; }

    public override void SyncWithSource(DnDContext context)
    {
        if (Enemy == null)
            Enemy = context.Enemies.Find(EnemyId);

        if (Enemy != null)
        {
            Enemy.CurrentHitPoints = CurrentHitPoints;
            context.Entry(Enemy).State = EntityState.Modified;
        }
    }
}

public enum ParticipantType
{
    Player,
    Npc,
    Enemy
}