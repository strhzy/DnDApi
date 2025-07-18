using System.ComponentModel.DataAnnotations;

namespace DnDAPI.Models;

public class Enemy
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [StringLength(100)]
        public string? Slug { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Size { get; set; }

        [StringLength(50)]
        public string? Type { get; set; }

        [StringLength(50)]
        public string? Subtype { get; set; }

        [StringLength(100)]
        public string? Group { get; set; }

        [StringLength(50)]
        public string? Alignment { get; set; }

        public int? ArmorClass { get; set; }

        [StringLength(100)]
        public string? ArmorDescription { get; set; }

        public int? HitPoints { get; set; }

        [StringLength(50)]
        public string? HitDice { get; set; }

        public Dictionary<string, string>? Speed { get; set; }

        public int? Strength { get; set; }

        public int? Dexterity { get; set; }

        public int? Constitution { get; set; }

        public int? Intelligence { get; set; }

        public int? Wisdom { get; set; }

        public int? Charisma { get; set; }

        public int? Perception { get; set; }

        public Dictionary<string, int>? Skills { get; set; }

        [StringLength(500)]
        public string? DamageVulnerabilities { get; set; }

        [StringLength(500)]
        public string? DamageResistances { get; set; }

        [StringLength(500)]
        public string? DamageImmunities { get; set; }

        [StringLength(500)]
        public string? ConditionImmunities { get; set; }

        [StringLength(500)]
        public string? Senses { get; set; }

        [StringLength(500)]
        public string? Languages { get; set; }

        [StringLength(50)]
        public string? ChallengeRating { get; set; }

        public double? ChallengeRatingValue { get; set; }

        public List<Attack> Attacks { get; set; } = new();

        [StringLength(1000)]
        public string? LegendaryDescription { get; set; }

        public List<SpecialAbility> SpecialAbilities { get; set; } = new();

        public List<string> SpellList { get; set; } = new();
    }