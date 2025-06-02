using code_first.Entities;
using Microsoft.EntityFrameworkCore;

namespace code_first;

public class AppDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Server=localhost;Database=db_versioning_efcore;User Id=sa;Password=Passw0rd!;Encrypt=False;";
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Home Appliances" }
        );

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("ProductItems");         // change table name
            entity.Property(p => p.Name)
                  .HasColumnName("ProductName")     // change column name
                  .HasMaxLength(100)                // max length
                  .IsRequired();                    // not null

            entity.HasData(                         // seeding data
                new Product { Id = 1, Name = "Laptop", Price = 1200.50M, CategoryId = 1 },
                new Product { Id = 2, Name = "Vacuum Cleaner", Price = 300.00M, CategoryId = 2 }
            );
            entity.HasIndex(p => p.Name).IsUnique();    // uk
            entity.HasQueryFilter(p => !p.IsDeleted);   // query for soft delete
        });

        base.OnModelCreating(modelBuilder);
    }
}
