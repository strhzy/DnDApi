using Microsoft.EntityFrameworkCore;

namespace DnDAPI.Models
{
    public class DnDContext : DbContext
    {
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<StoryElement> StoryElements { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Attack> Attacks { get; set; }
        public DbSet<Combat> Combats { get; set; }
        public DbSet<CombatParticipant> CombatParticipants { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<NPC> NPCs { get; set; }
        public DbSet<PlayerCharacter> PlayerCharacters { get; set; }
        public DbSet<Enemy> Enemies { get; set; }
        public DbSet<SpecialAbility> SpecialAbilities { get; set; }

        public DnDContext(DbContextOptions<DnDContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Конфигурация User
            modelBuilder.Entity<User>()
                .HasMany(u => u.Characters)
                .WithOne()
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Campaigns)
                .WithOne()
                .HasForeignKey(c => c.MasterId);
            
            modelBuilder.Entity<Campaign>()
                .HasMany(c => c.PlotItems)
                .WithOne(s => s.Campaign)
                .HasForeignKey(s => s.CampaignId);

            modelBuilder.Entity<Combat>()
                .HasMany(c => c.Participants)
                .WithOne()
                .HasForeignKey(p => p.Id);

            modelBuilder.Entity<PlayerCharacter>()
                .HasMany(p => p.Attacks)
                .WithOne()
                .HasForeignKey(a => a.PlayerCharacterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enemy>()
                .HasMany(e => e.Attacks)
                .WithOne()
                .HasForeignKey(a => a.EnemyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enemy>()
                .HasMany(e => e.SpecialAbilities)
                .WithOne(s => s.Enemy)
                .HasForeignKey(s => s.EnemyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Конвертация Dictionary для PostgreSQL
            modelBuilder.Entity<Enemy>()
                .Property(e => e.Speed)
                .HasColumnType("jsonb");

            modelBuilder.Entity<Enemy>()
                .Property(e => e.Skills)
                .HasColumnType("jsonb");
        }
    }
}