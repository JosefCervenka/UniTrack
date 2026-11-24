using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Item> Item { get; set; }
    public DbSet<Status> Status { get; set; }
    public DbSet<User> User { get; set; }
}