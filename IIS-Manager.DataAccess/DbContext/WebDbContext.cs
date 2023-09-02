using IIS_Manager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IIS_Manager.DataAccess.DbContext;

public class WebDbContext : IdentityDbContext<WebUser>
{
    public WebDbContext(DbContextOptions<WebDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IisServer>()
            .ToTable("IisServer");
        modelBuilder.Entity<Favorite>()
            .ToTable("Favorite");
        modelBuilder.Entity<Log>()
            .ToTable("Log");

        base.OnModelCreating(modelBuilder);
    }
}
