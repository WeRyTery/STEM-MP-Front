using Microsoft.EntityFrameworkCore;
using TechTrend.Api.Models;

namespace TechTrend.Api.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Technology> Technologies => Set<Technology>();
        public DbSet<TrendSnapshot> TrendSnapshots => Set<TrendSnapshot>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Technology>().HasIndex(t => t.Name).IsUnique();

            modelBuilder.Entity<TrendSnapshot>().HasIndex(s => new { s.TechnologyId, s.Source, s.Period, s.CollectedAt });

            modelBuilder.Entity<TrendSnapshot>().HasOne(s => s.Technology).WithMany(t => t.Snapshots).HasForeignKey(s => s.TechnologyId);
        }
    }
}
