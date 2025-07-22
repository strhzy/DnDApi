using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DnDAPI.Models;

public class Campaign
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [ForeignKey("Master")]
    public Guid MasterId { get; set; }
    
    public User Master { get; set; }

    public List<StoryElement> PlotItems { get; set; } = new();
    
    public ICollection<PlayerCharacter> PlayerCharacters { get; set; } = new List<PlayerCharacter>();
}