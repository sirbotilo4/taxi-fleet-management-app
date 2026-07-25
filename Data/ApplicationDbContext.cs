using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TaxiFleetManager.Models;

namespace TaxiFleetManager.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<OwnerProfile> OwnerProfiles { get; set; }

    public virtual DbSet<TaxiRoute> Routes { get; set; }

    public virtual DbSet<Trip> Trips { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.DriverId).HasName("PK__Drivers__F1B1CD04D650BCA8");

            entity.Property(e => e.ContactNumber).HasMaxLength(20);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.HireDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LicenseNumber).HasMaxLength(50);
            entity.Property(e => e.Pin).HasMaxLength(4);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.ExpenseId).HasName("PK__Expenses__1445CFD3112F3C3C");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.ExpenseDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpenseType).HasMaxLength(50);

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("FK_Expenses_Vehicles");
        });

        modelBuilder.Entity<OwnerProfile>(entity =>
        {
            entity.HasKey(e => e.OwnerId).HasName("PK__OwnerPro__819385B8E79BC47E");

            entity.ToTable("OwnerProfile");

            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.BankName).HasMaxLength(100);
            entity.Property(e => e.BranchCode).HasMaxLength(20);
            entity.Property(e => e.BusinessName).HasMaxLength(150);
            entity.Property(e => e.ContactNumber).HasMaxLength(20);
            entity.Property(e => e.FullName).HasMaxLength(150);
        });

        modelBuilder.Entity<TaxiRoute>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("PK__Routes__80979B4D7C393F17");

            entity.Property(e => e.EndPoint).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.RouteType)
                .HasMaxLength(20)
                .HasDefaultValue("Local");
            entity.Property(e => e.StandardFare).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.StartPoint).HasMaxLength(100);
        });

        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.TripId).HasName("PK__Trips__51DC713E46A6526B");

            entity.Property(e => e.AmountCollected).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .HasDefaultValue("Cash");
            entity.Property(e => e.PaymentReference).HasMaxLength(100);
            entity.Property(e => e.TripDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Driver).WithMany(p => p.Trips)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Trips_Drivers");

            entity.HasOne(d => d.Route).WithMany(p => p.Trips)
                .HasForeignKey(d => d.RouteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Trips_Routes");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.Trips)
                .HasForeignKey(d => d.VehicleId)
                .HasConstraintName("FK_Trips_Vehicles");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicles__476B5492D6037235");

            entity.HasIndex(e => e.RegistrationNumber, "UQ__Vehicles__E886460274FEC7ED").IsUnique();

            entity.Property(e => e.Make).HasMaxLength(50);
            entity.Property(e => e.Model).HasMaxLength(50);
            entity.Property(e => e.RegistrationNumber).HasMaxLength(20);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.Driver).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Vehicles_Drivers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
