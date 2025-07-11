using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DnDAPI.Models;

public class Client
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(50)]
    public string Ip { get; set; } = string.Empty;

    public DateTime ConnectedTime { get; set; } = DateTime.UtcNow;

    [NotMapped]
    [JsonIgnore]
    public System.Net.WebSockets.WebSocket? Socket { get; set; }
}