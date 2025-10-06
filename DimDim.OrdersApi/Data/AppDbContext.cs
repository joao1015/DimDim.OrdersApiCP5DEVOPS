using Microsoft.EntityFrameworkCore;
using DimDim.OrdersApi.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DimDim.OrdersApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ServiceOrder> ServiceOrders => Set<ServiceOrder>();
    public DbSet<ServicePart> ServiceParts => Set<ServicePart>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<ServiceOrder>()
         .HasMany(o => o.Parts)
         .WithOne(p => p.ServiceOrder!)
         .HasForeignKey(p => p.ServiceOrderId)
         .OnDelete(DeleteBehavior.Cascade);

        b.Entity<ServiceOrder>()
         .Property(o => o.TotalCost)
         .HasColumnType("decimal(18,2)");
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<ServiceOrder>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                var sum = (entry.Entity.Parts ?? []).Sum(p => p.Quantity * p.UnitPrice);
                entry.Entity.TotalCost = sum;
            }
        }
        return base.SaveChangesAsync(ct);
    }
}
