using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<LostItem> LostItem { get; set; }

    public DbSet<FoundItem> FoundItem { get; set; }

    public DbSet<Status> Status { get; set; }
    public DbSet<User> User { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<LostItem>(entity =>
        {
            entity.Property(x => x.Embedding)
                .HasColumnType("vector(384)");
            
            entity.HasIndex(x => x.Embedding)
                .HasMethod("hnsw")
                .HasOperators("vector_cosine_ops");
        });
        
        modelBuilder.Entity<Status>().HasData(
            new Status { Id = (int)StatusEnum.Lost, DisplayName = "Lost" },
            new Status { Id = (int)StatusEnum.Found, DisplayName = "Found" },
            new Status { Id = (int)StatusEnum.Returned, DisplayName = "Returned" },
            new Status { Id = (int)StatusEnum.Expired, DisplayName = "Expired" }
        );
    }
}