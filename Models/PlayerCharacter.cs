using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DnDAPI.Models;

public class PlayerCharacter
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [JsonIgnore]
        public ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string PlayerName { get; set; } = string.Empty;

        [StringLength(50)]
        public string ClassType { get; set; } = string.Empty;

        [StringLength(50)]
        public string Background { get; set; } = string.Empty;

        [StringLength(50)]
        public string Race { get; set; } = string.Empty;

        [StringLength(50)]
        public string Alignment { get; set; } = string.Empty;
        
        [Required]
        [ForeignKey("User")]
        public Guid UserId {get; set; }

        [JsonIgnore]
        public User? User;

        public int ExperiencePoints { get; set; }

        public int Level { get; set; } = 1;

        public bool Inspiration { get; set; }

        // Валюта
        public int CopperPieces { get; set; }

        public int SilverPieces { get; set; }

        public int ElectrumPieces { get; set; }

        public int GoldPieces { get; set; }

        public int PlatinumPieces { get; set; }

        // Черты характера
        [StringLength(500)]
        public string PersonalityTraits { get; set; } = string.Empty;

        [StringLength(500)]
        public string Ideals { get; set; } = string.Empty;

        [StringLength(500)]
        public string Bonds { get; set; } = string.Empty;

        [StringLength(500)]
        public string Flaws { get; set; } = string.Empty;

        // Характеристики
        public int Strength { get; set; }

        public int Dexterity { get; set; }

        public int Constitution { get; set; }

        public int Intelligence { get; set; }

        public int Wisdom { get; set; }

        public int Charisma { get; set; }

        // Proficiency бонус
        public int ProficiencyBonus { get; set; }

        // Saving throws proficiencies
        public bool SavingThrowStrengthProficiency { get; set; }

        public bool SavingThrowDexterityProficiency { get; set; }

        public bool SavingThrowConstitutionProficiency { get; set; }

        public bool SavingThrowIntelligenceProficiency { get; set; }

        public bool SavingThrowWisdomProficiency { get; set; }

        public bool SavingThrowCharismaProficiency { get; set; }

        // Saving throws (вычисляемые, но храним в базе для упрощения)
        public int SavingThrowStrength { get; set; }

        public int SavingThrowDexterity { get; set; }

        public int SavingThrowConstitution { get; set; }

        public int SavingThrowIntelligence { get; set; }

        public int SavingThrowWisdom { get; set; }

        public int SavingThrowCharisma { get; set; }

        // Навыки
        public int Acrobatics { get; set; }

        public int AnimalHandling { get; set; }

        public int Arcana { get; set; }

        public int Athletics { get; set; }

        public int Deception { get; set; }

        public int History { get; set; }

        public int Insight { get; set; }

        public int Intimidation { get; set; }

        public int Investigation { get; set; }

        public int Medicine { get; set; }

        public int Nature { get; set; }

        public int Perception { get; set; }

        public int Performance { get; set; }

        public int Persuasion { get; set; }

        public int Religion { get; set; }

        public int SleightOfHand { get; set; }

        public int Stealth { get; set; }

        public int Survival { get; set; }

        public int PassiveWisdom { get; set; }

        // Боевая статистика
        public int ArmorClass { get; set; }

        public int Initiative { get; set; }

        public int Speed { get; set; }

        public int MaxHitPoints { get; set; }

        public int CurrentHitPoints { get; set; }

        public int TemporaryHitPoints { get; set; }

        [StringLength(50)]
        public string HitDice { get; set; } = string.Empty;

        public int DeathSaveSuccesses { get; set; }

        public int DeathSaveFailures { get; set; }

        // Атаки и заклинания
        public List<Attack> Attacks { get; set; } = new();

        [StringLength(1000)]
        public string FeaturesAndTraits { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Equipment { get; set; } = string.Empty;

        [StringLength(1000)]
        public string ProficienciesAndLanguages { get; set; } = string.Empty;

        // Логика для saving throws
        [NotMapped]
        [JsonIgnore]
        private readonly Dictionary<string, (int Score, bool IsProficient, string SavingThrowProperty)> _abilityScores = new()
        {
            { nameof(Strength), (0, false, nameof(SavingThrowStrength)) },
            { nameof(Dexterity), (0, false, nameof(SavingThrowDexterity)) },
            { nameof(Constitution), (0, false, nameof(SavingThrowConstitution)) },
            { nameof(Intelligence), (0, false, nameof(SavingThrowIntelligence)) },
            { nameof(Wisdom), (0, false, nameof(SavingThrowWisdom)) },
            { nameof(Charisma), (0, false, nameof(SavingThrowCharisma)) }
        };

        public void UpdateSavingThrows()
        {
            foreach (var ability in _abilityScores.Keys)
            {
                var (score, isProficient, savingThrowProperty) = _abilityScores[ability];
                int value = CalculateSavingThrow(score, isProficient);
                typeof(PlayerCharacter).GetProperty(savingThrowProperty)!.SetValue(this, value);
            }
        }

        private int CalculateSavingThrow(int abilityScore, bool isProficient)
        {
            int modifier = (abilityScore - 10) / 2;
            return isProficient ? modifier + ProficiencyBonus : modifier;
        }

        public void UpdateAbilityScore(string abilityName, int score)
        {
            if (_abilityScores.ContainsKey(abilityName))
            {
                var (oldScore, isProficient, savingThrowProperty) = _abilityScores[abilityName];
                _abilityScores[abilityName] = (score, isProficient, savingThrowProperty);
                typeof(PlayerCharacter).GetProperty(abilityName)!.SetValue(this, score);
                int value = CalculateSavingThrow(score, isProficient);
                typeof(PlayerCharacter).GetProperty(savingThrowProperty)!.SetValue(this, value);
            }
        }

        public void UpdateProficiency(string abilityName, bool isProficient)
        {
            if (_abilityScores.ContainsKey(abilityName))
            {
                var (score, oldIsProficient, savingThrowProperty) = _abilityScores[abilityName];
                _abilityScores[abilityName] = (score, isProficient, savingThrowProperty);
                typeof(PlayerCharacter).GetProperty($"SavingThrow{abilityName}Proficiency")!.SetValue(this, isProficient);
                int value = CalculateSavingThrow(score, isProficient);
                typeof(PlayerCharacter).GetProperty(savingThrowProperty)!.SetValue(this, value);
            }
        }

        public void UpdateProficiencyBonus(int value)
        {
            ProficiencyBonus = value;
            UpdateSavingThrows();
        }
    }