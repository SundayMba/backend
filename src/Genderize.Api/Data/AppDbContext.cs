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
            entity.HasIndex(profile => profile.Name).IsUnique();
            entity.HasIndex(profile => profile.Gender);
            entity.HasIndex(profile => profile.AgeGroup);
            entity.HasIndex(profile => profile.CountryId);
            entity.HasIndex(profile => profile.CreatedAt);

            entity.Property(profile => profile.Name).HasMaxLength(100).IsRequired();
            entity.Property(profile => profile.Gender).HasMaxLength(20).IsRequired();
            entity.Property(profile => profile.AgeGroup).HasMaxLength(20).IsRequired();
            entity.Property(profile => profile.CountryId).HasMaxLength(2).IsRequired();
            entity.Property(profile => profile.CountryName).HasMaxLength(100).IsRequired();
            entity.Property(profile => profile.GenderProbability).HasPrecision(5, 4);
            entity.Property(profile => profile.CountryProbability).HasPrecision(5, 4);
            entity.Property(profile => profile.CreatedAt).HasColumnType("timestamp with time zone");
        });
    }
}
