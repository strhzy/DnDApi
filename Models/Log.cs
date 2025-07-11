using System.ComponentModel.DataAnnotations;

namespace DnDAPI.Models;

public class Log
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(50)]
    public string Tag { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Message { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.UtcNow;
}