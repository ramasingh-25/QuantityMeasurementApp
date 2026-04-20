using Microsoft.EntityFrameworkCore;
using QuantityMeasurementModelLayer.Entities;

namespace QuantityMeasurementRepositoryLayer.Context;

public class QuantityMeasurementDbContext : DbContext
{
    public QuantityMeasurementDbContext(DbContextOptions<QuantityMeasurementDbContext> options)
        : base(options)
    {
    }

    public DbSet<QuantityMeasurementEntity> QuantityMeasurements { get; set; }
    public DbSet<UserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure QuantityMeasurements table
        modelBuilder.Entity<QuantityMeasurementEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstUnit).IsRequired();
            entity.Property(e => e.SecondUnit).IsRequired();
            entity.Property(e => e.Operation).IsRequired();
            entity.Property(e => e.MeasurementType).IsRequired();
        });

        // Configure Users table
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.Password).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });
    }
}