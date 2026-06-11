using Microsoft.EntityFrameworkCore;
using Model;

namespace API.Data;

public class FootballStatsContext : DbContext
{
    public FootballStatsContext(DbContextOptions<FootballStatsContext> options) : base(options) { }

    public DbSet<PlayerStat> PlayerStats => Set<PlayerStat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ensure unique lookups per player per season
        modelBuilder.Entity<PlayerStat>()
            .HasIndex(p => new { p.PlayerId, p.Season })
            .IsUnique();
    }
}
