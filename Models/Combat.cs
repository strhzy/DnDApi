using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DnDAPI.Models;

public class Combat
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [ForeignKey("Campaign")]
    public Guid CampaignId { get; set; }
    
    [JsonIgnore]
    [ForeignKey("CampaignId")]
    public Campaign? Campaign { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public List<CombatParticipant> Participants { get; set; } = new();

    public int CurrentRound { get; set; } = 1;

    public int CurrentTurnIndex { get; set; } = 0;

    [NotMapped]
    public CombatParticipant? CurrentParticipant
    {
        get => Participants != null && CurrentTurnIndex >= 0 && CurrentTurnIndex < Participants.Count
            ? Participants[CurrentTurnIndex]
            : null;
    }
}