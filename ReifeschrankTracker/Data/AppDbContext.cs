using Microsoft.EntityFrameworkCore;
using ReifeschrankTracker.Models;
using System;
using System.IO;

namespace ReifeschrankTracker.Data;

public class AppDbContext : DbContext
{
    public DbSet<Charge> Chargen { get; set; }
    public DbSet<Messung> Messungen { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ReifeschrankTracker");
        Directory.CreateDirectory(folder);
        var dbPath = Path.Combine(folder, "reifen.db");
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Charge>()
            .HasMany(c => c.Messungen)
            .WithOne(m => m.Charge)
            .HasForeignKey(m => m.ChargeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
