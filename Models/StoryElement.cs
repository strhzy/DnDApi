using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DnDAPI.Models;

public class StoryElement
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [ForeignKey("Campaign")]
    public Guid CampaignId { get; set; }

    [ForeignKey("CampaignId")]
    public Campaign? Campaign { get; set; }
}