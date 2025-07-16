using ECS_Logistics.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ECS_Logistics.Data;

public class MySqlDbContext(DbContextOptions<MySqlDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    
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
    }
}