using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<LostItem> LostItem { get; set; }

    public DbSet<FoundItem> FoundItem { get; set; }

    public DbSet<Status> Status { get; set; }
    public DbSet<User> User { get; set; }
}