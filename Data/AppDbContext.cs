using Infomatrix.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Infomatrix.Api.Data;

public class AppDbContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
}
