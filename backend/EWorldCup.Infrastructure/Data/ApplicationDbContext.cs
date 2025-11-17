using EWorldCup.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EWorldCup.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<GameRound> GameRounds { get; set; }
        public DbSet<PlayerScore> PlayerScores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure player entity
            ConfigurePlayer(modelBuilder);

            // Configure tournament entity
            ConfigureTournament(modelBuilder);

            // Configure match entity
            ConfigureMatch(modelBuilder);

            // Configure game round entity
            ConfigureGameRound(modelBuilder);

            // Configure player score entity
            ConfigurePlayerScore(modelBuilder);
        }
        private static void ConfigurePlayer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Uid).IsRequired();

                entity.HasIndex(p => p.Uid).IsUnique();
            });
        }
        private static void ConfigureTournament(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.HasKey(t => t.Id);

                entity.Property(t => t.PlayerName).IsRequired().HasMaxLength(100);
                entity.Property(t => t.TotalPlayers).IsRequired();
                entity.Property(t => t.CurrentRound).HasDefaultValue(0);
                entity.Property(t => t.Status).IsRequired().HasConversion<int>();
                entity.Property(t => t.StartedAt).IsRequired();
                entity.Property(t => t.CompletedAt).IsRequired(false);

                // Relationships
                entity.HasMany(t => t.Matches).WithOne().HasForeignKey(m => m.TournamentId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(t => t.Scoreboard).WithOne().HasForeignKey(s => s.TournamentId).OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(t => t.Status);
                entity.HasIndex(t => t.StartedAt);
            });
        }

        private static void ConfigureMatch(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.TournamentId).IsRequired();
                entity.Property(m => m.Round).IsRequired();
                entity.Property(m => m.Player1Index).IsRequired();
                entity.Property(m => m.Player1Name).IsRequired().HasMaxLength(100);
                entity.Property(m => m.Player2Index).IsRequired();
                entity.Property(m => m.Player2Name).IsRequired().HasMaxLength(100);
                entity.Property(m => m.IsPlayerMatch).IsRequired().HasDefaultValue(false);
                entity.Property(m => m.Status).IsRequired().HasConversion<int>();
                entity.Property(m => m.Player1Wins).IsRequired().HasDefaultValue(0);
                entity.Property(m => m.Player2Wins).IsRequired().HasDefaultValue(0);
                entity.Property(m => m.WinnerIndex).IsRequired(false);

                // Relationships
                entity.HasMany(m => m.GameRounds).WithOne().HasForeignKey(g => g.MatchId).OnDelete(DeleteBehavior.Cascade);

                // Indexes
                entity.HasIndex(m => new { m.TournamentId, m.Round });
                entity.HasIndex(m => m.IsPlayerMatch);
                entity.HasIndex(m => m.Status);
            });
        }

        private static void ConfigureGameRound(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameRound>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.MatchId).IsRequired();
                entity.Property(g => g.RoundNumber).IsRequired();
                entity.Property(g => g.Player1Choice).IsRequired().HasConversion<int>();
                entity.Property(g => g.Player2Choice).IsRequired().HasConversion<int>();
                entity.Property(g => g.Result).IsRequired().HasConversion<int>();
                entity.Property(g => g.PlayedAt).IsRequired();

                // Indexes
                entity.HasIndex(g => new { g.MatchId, g.RoundNumber });
            });
        }

        private static void ConfigurePlayerScore(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerScore>(entity =>
            {
                entity.HasKey(s => s.Id);

                entity.Property(s => s.TournamentId).IsRequired();
                entity.Property(s => s.PlayerIndex).IsRequired();
                entity.Property(s => s.PlayerName).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Wins).IsRequired().HasDefaultValue(0);
                entity.Property(s => s.Losses).IsRequired().HasDefaultValue(0);
                entity.Property(s => s.Points).IsRequired().HasDefaultValue(0);

                entity.Ignore(s => s.MatchesPlayed);

                // Indexes
                entity.HasIndex(s => new { s.TournamentId, s.PlayerIndex }).IsUnique();
                entity.HasIndex(s => s.Points);
            });
        }
    }
}
