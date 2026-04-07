using Microsoft.EntityFrameworkCore;
using PalletApp.Domain.Entities;

namespace PalletApp.Infrastructure;

public class PalletAppDbContext : DbContext
{
    public PalletAppDbContext(DbContextOptions<PalletAppDbContext> options) : base(options) { }

    public DbSet<BinLocation> BinLocations { get; set; }
    public DbSet<Pallet> Pallets { get; set; }
    public DbSet<Batch> Batches { get; set; }
    public DbSet<PalletBatch> PalletBatches { get; set; }
    public DbSet<PrintEvent> PrintEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BinLocation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LocationName).HasMaxLength(50).IsRequired();
            entity.HasMany(e => e.Pallets)
                  .WithOne()
                  .HasForeignKey(p => p.BinLocationId);
        });

        modelBuilder.Entity<Pallet>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PalletNo).HasMaxLength(50);
            entity.HasMany(e => e.PalletBatches)
                  .WithOne()
                  .HasForeignKey(pb => pb.PalletId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.PrintEvents)
                  .WithOne()
                  .HasForeignKey(pe => pe.PalletId);
        });

        modelBuilder.Entity<Batch>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BatchId).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<PalletBatch>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Batch)
                  .WithMany()
                  .HasForeignKey(e => e.BatchId);
        });

        modelBuilder.Entity<PrintEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrintType).HasMaxLength(20);
        });

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Sample BinLocations
        var bin1Id = Guid.NewGuid();
        var bin2Id = Guid.NewGuid();

        modelBuilder.Entity<BinLocation>().HasData(
            new { Id = bin1Id, LocationName = "BIN-A01", Description = "Khu vực A - Hàng 01", IsActive = true },
            new { Id = bin2Id, LocationName = "BIN-A02", Description = "Khu vực A - Hàng 02", IsActive = true }
        );

        // Sample Batches
        for (int i = 1; i <= 20; i++)
        {
            modelBuilder.Entity<Batch>().HasData(new
            {
                Id = Guid.NewGuid(),
                BatchId = $"B2026-{i:D3}",
                ProductionLine = i <= 10 ? "LINE-01" : "LINE-02",
                ProductionDate = DateTime.Now.AddDays(-i),
                Status = "Available",
                CreatedDate = DateTime.Now
            });
        }
    }
}
