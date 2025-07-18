using ECS_Logistics.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ECS_Logistics.Data;

public class MySqlDbContext(DbContextOptions<MySqlDbContext> options) : DbContext(options)
{
    public DbSet<Product> Product { get; set; }
    public DbSet<DeliveryAgent> DeliveryAgents { get; set; }
    public DbSet<OrderReturn> OrderReturns { get; set; }
    public DbSet<DeliveryHub> DeliveryHubs { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("product");
            entity.Property(p => p.DateAdded)
                .ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            entity.Property(p => p.ProductName).IsRequired();
            entity.Property(p => p.ProductPrice).IsRequired();
            entity.Property(p => p.ProductQuantity).IsRequired();
        });
        
        modelBuilder.Entity<DeliveryAgent>(entity =>
        {
            entity.ToTable("delivery_agents");
            entity.Property(e => e.DateAdded)
                .ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            entity.Property(e => e.DeliveryAgentName).IsRequired();
            entity.Property(e => e.ContactNumber).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.Password).IsRequired();
            entity.Property(e => e.AvailabilityStatus).IsRequired();
            entity.Property(e => e.TotalDeliveries).IsRequired();
            entity.Property(e => e.ServingArea).IsRequired();
            entity.Property(e => e.DateAdded).IsRequired();
            entity.Property(e => e.DateModified).IsRequired();
        });
        
        modelBuilder.Entity<OrderReturn>(entity =>
        {
            entity.ToTable("order_returns");
            entity.Property(e => e.DateAdded)
                .ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            entity.Property(e => e.OrderId).IsRequired();
            entity.Property(e => e.OrderTrackingId).IsRequired();
            entity.Property(e => e.DeliveryAgentId).IsRequired();
            entity.Property(e => e.OrderItemId).IsRequired();
            entity.Property(e => e.ReturnReasonCategoryId).IsRequired();
            entity.Property(e => e.DateAdded).IsRequired();
            entity.Property(e => e.DateModified).IsRequired();
            entity.Property(e => e.ReturnPaymentSourceId).IsRequired();
        });
        
        modelBuilder.Entity<DeliveryHub>(entity =>
        {
            entity.ToTable("delivery_hubs");
            entity.Property(e => e.DeliveryHubName).IsRequired();
            entity.Property(e => e.DeliveryHubAddressId).IsRequired();
        });
    }
}