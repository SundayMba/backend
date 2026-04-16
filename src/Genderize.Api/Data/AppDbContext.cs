using Genderize.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Genderize.Api.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Profile> Profiles => Set<Profile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Profile>(entity =>
        {
            entity.ToTable("profiles");
            entity.HasKey(profile => profile.Id);
            entity.HasIndex(profile => profile.NormalizedName).IsUnique();

            entity.Property(profile => profile.Name).HasMaxLength(100).IsRequired();
            entity.Property(profile => profile.NormalizedName).HasMaxLength(100).IsRequired();
            entity.Property(profile => profile.Gender).HasMaxLength(20).IsRequired();
            entity.Property(profile => profile.AgeGroup).HasMaxLength(20).IsRequired();
            entity.Property(profile => profile.CountryId).HasMaxLength(3).IsRequired();
            entity.Property(profile => profile.CreatedAt).HasColumnType("timestamp with time zone");
        });
    }
}
