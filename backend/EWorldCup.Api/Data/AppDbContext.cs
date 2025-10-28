using EWorldCup.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EWorldCup.Api.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Participant> Participants => Set<Participant>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Participant>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedNever();
                e.Property(x => x.Uid).IsRequired();
                e.HasIndex(x => x.Uid).IsUnique();
                e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            });

            base.OnModelCreating(b);
        }
    }
}
