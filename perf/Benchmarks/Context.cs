using Microsoft.EntityFrameworkCore;

namespace ResetDBIsolationLevel.Benchmarks;

public class Context : DbContext
{
    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }

    public DbSet<Blog> Blogs { get; set; } = default;
}