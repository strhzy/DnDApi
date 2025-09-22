using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DnDAPI.Models;

public class CombatLog
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? CombatId { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    public Guid SourceId { get; set; }
    public Guid TargetId { get; set; }
    public int? Damage { get; set; }
    public string? Message { get; set; }
    
    [ForeignKey("CombatId")]
    public virtual Combat? Combat { get; set; }

    [ForeignKey("SourceId")]
    public virtual CombatParticipant Source { get; set; } = null!;

    [ForeignKey("TargetId")]
    public virtual CombatParticipant Target { get; set; } = null!;
}