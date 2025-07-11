using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DnDAPI.Models;

public class SpecialAbility
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [ForeignKey("Enemy")]
    public Guid EnemyId { get; set; }

    [JsonIgnore]
    public Enemy Enemy { get; set; } = null!;
}