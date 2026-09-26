using Microsoft.EntityFrameworkCore;

namespace TodoApi;

public class AppDbContext : DbContext
{
    public DbSet<TaskItem> TaskItems { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
}