using GiveMap_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace GiveMap_Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Location>()
            .Property(e => e.ImageUrls)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        
        modelBuilder.Entity<LocationFeedback>()
            .HasOne(lf => lf.User)
            .WithMany()
            .HasForeignKey(lf => lf.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<Donation>()
            .HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }

    

    // Add DbSets for your entities here
    public DbSet<User> Users { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<LocationFeedback> LocationFeedbacks { get; set; }
    public DbSet<Need> Needs { get; set; }
    public DbSet<Donation> Donations { get; set; }
}