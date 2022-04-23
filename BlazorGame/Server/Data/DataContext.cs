using BlazorGame.Shared;
using Microsoft.EntityFrameworkCore;

namespace BlazorGame.Server.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Unit>? Units { get; set; }

    public DbSet<User>? Users { get; set; }
}
