using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

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
    
    [JsonIgnore]
    [ForeignKey("CombatId")]
    public virtual Combat? Combat { get; set; }

    [JsonIgnore]
    [ForeignKey("SourceId")]
    public virtual CombatParticipant? Source { get; set; }
    
    [JsonIgnore]
    [ForeignKey("TargetId")]
    public virtual CombatParticipant? Target { get; set; }
}