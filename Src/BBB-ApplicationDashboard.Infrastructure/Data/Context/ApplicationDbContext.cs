using System.Text.Json;
using BBB_ApplicationDashboard.Domain;
using BBB_ApplicationDashboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BBB_ApplicationDashboard.Infrastructure.Data.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Accreditation> Accreditations { get; set; }
    public DbSet<ActivityEvent> ActivityEvents => Set<ActivityEvent>();

    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // 👈 optional: camelCase in DB JSON
            WriteIndented = false,
        };

        // List<string> → JSONB
        modelBuilder
            .Entity<Accreditation>()
            .Property(a => a.SocialMediaLinks)
            .HasConversion(
                v => JsonSerializer.Serialize(v, options), // serialize before save
                v => JsonSerializer.Deserialize<List<string>>(v, options) ?? new List<string>() // deserialize after load
            )
            .HasColumnType("jsonb");

        modelBuilder
            .Entity<Accreditation>()
            .Property(a => a.PrimaryContactTypes)
            .HasConversion(
                v => JsonSerializer.Serialize(v, options),
                v => JsonSerializer.Deserialize<List<string>>(v, options) ?? new List<string>()
            )
            .HasColumnType("jsonb");

        modelBuilder
            .Entity<Accreditation>()
            .Property(a => a.SecondaryContactTypes)
            .HasConversion(
                v => JsonSerializer.Serialize(v, options),
                v => JsonSerializer.Deserialize<List<string>>(v, options) ?? new List<string>()
            )
            .HasColumnType("jsonb");

        // List<License> → JSONB
        modelBuilder
            .Entity<Accreditation>()
            .Property(a => a.Licenses)
            .HasConversion(
                v => JsonSerializer.Serialize(v, options),
                v => JsonSerializer.Deserialize<List<License>>(v, options) ?? new List<License>()
            )
            .HasColumnType("jsonb");

        modelBuilder.Entity<Accreditation>().HasIndex(a => a.ApplicationNumber).IsUnique();
    }
}
